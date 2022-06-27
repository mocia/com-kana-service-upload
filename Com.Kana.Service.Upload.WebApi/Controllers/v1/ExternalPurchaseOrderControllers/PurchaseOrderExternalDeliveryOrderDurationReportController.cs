﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Com.Kana.Service.Upload.Lib.Facades;
using Com.Kana.Service.Upload.Lib.Facades.ExternalPurchaseOrderFacade;
using Com.Kana.Service.Upload.Lib.Services;
using Com.Kana.Service.Upload.Lib.ViewModels.ExternalPurchaseOrderViewModel;
using Com.Kana.Service.Upload.WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Com.Kana.Service.Upload.WebApi.Controllers.v1.ExternalPurchaseOrderControllers
{
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("v{version:apiVersion}/duration-report/purchase-order-external-to-delivery-order-duration-reports")]
    [Authorize]
    public class PurchaseOrderExternalDeliveryOrderDurationReportController : Controller
    {
        private string ApiVersion = "1.0.0";
        private readonly IMapper _mapper;
        private readonly ExternalPurchaseOrderFacade _facade;
        private readonly IdentityService identityService;
        public PurchaseOrderExternalDeliveryOrderDurationReportController(IMapper mapper, ExternalPurchaseOrderFacade facade, IdentityService identityService)
        {
            _mapper = mapper;
            _facade = facade;
            this.identityService = identityService;
        }

        [HttpGet]
        public IActionResult GetReport(string unitId, string duration, DateTime? dateFrom, DateTime? dateTo, int page, int size, string Order = "{}")
        {
            int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
            string accept = Request.Headers["Accept"];

            try
            {

                var data = _facade.GetEPODODurationReport(unitId, duration, dateFrom, dateTo, page, size, Order, offset);

                return Ok(new
                {
                    apiVersion = ApiVersion,
                    data = data.Item1,
                    info = new { total = data.Item2 },
                    message = General.OK_MESSAGE,
                    statusCode = General.OK_STATUS_CODE
                });
            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
        [HttpGet("download")]
        public IActionResult GetXls(string unitId, string duration, DateTime? dateFrom, DateTime? dateTo)
        {

            try
            {
                byte[] xlsInBytes;
                int offset = Convert.ToInt32(Request.Headers["x-timezone-offset"]);
                DateTime DateFrom = dateFrom == null ? new DateTime(1970, 1, 1) : Convert.ToDateTime(dateFrom);
                DateTime DateTo = dateTo == null ? DateTime.Now : Convert.ToDateTime(dateTo);

                var xls = _facade.GenerateExcelEPODODuration(unitId, duration, dateFrom, dateTo, offset);

                string filename = String.Format("Laporan Durasi PO Eksternal - Surat Jalan - {0}.xlsx", DateTime.UtcNow.ToString("ddMMyyyy"));

                xlsInBytes = xls.ToArray();
                var file = File(xlsInBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", filename);
                return file;

            }
            catch (Exception e)
            {
                Dictionary<string, object> Result =
                    new ResultFormatter(ApiVersion, General.INTERNAL_ERROR_STATUS_CODE, e.Message)
                    .Fail();
                return StatusCode(General.INTERNAL_ERROR_STATUS_CODE, Result);
            }
        }
    }
}