﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Com.Kana.Service.Upload.Lib.Facades.GarmentPurchasingExpedition
{
    public class GarmentInternNoteUpdateIsPphPaidDto
    {
        public long InternNoteId { get; set; }
        public string InternNoteNo { get; set; }
        public bool IsPphPaid { get; set; }
    }
}
