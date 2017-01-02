using System.Collections.Generic;

namespace TradeMessageGenerator
{
    public static class UtilityHelper
    {
        public static Dictionary<int, string> GetTradeCodeValues()
        {
            var tradeValues = new Dictionary<int, string>()
            {
                [9] = "BodyLength",
                [35] = "MsgType",
                [34] = "MsgSeqNum",
                [49] = "SenderCompID",
                [52] = "SendingTime",
                [56] = "TargetCompID",
                [58] = "Text",
                [131] = "QuoteReqID",
                [146] = "NoRelatedSym",
                [55] = "Symbol",
                [48] = "SecurityID",
                [22] = "SecurityIDSource",
                [460] = "Product",
                [167] = "SecurityType",
                [762] = "SecuritySubType",
                [541] = "MaturityDate",
                [1184] = "SecurityXMLLen",
                [1185] = "SecurityXML",
                [1186] = "SecurityXMLSchema",
                [54] = "Side",
                [38] = "OrderQty",
                [64] = "SettlDate",
                [15] = "Currency",
                [60] = " TransactTime",
                [221] = "BenchmarkCurveName",
                [222] = "BenchmarkCurvePoint",
                [423] = "PriceType",
                [66] = "ListID",
                [75] = "TradeDate",
                [828] = "TrdType",
                [10] = "CheckSum",
                [453] = "NoPartyIDs",
                [448] = "PartyID",
                [447] = "PartyIDSource",
                [452] = "PartyRole",
                [802] = "NoPartySubIDs",
                [523] = "PartySubID",
                [803] = "PartySubIDType",
                [20077] = "BookName",
                [20078] = "TraderList",
                [20079] = "TimeoutPeriod",
                [22001] = "NettingTradeID",
                [22005] = "CertaintyOfClearing",
                [20184] = "ClearingCategory",
                [5023] = "TickSize",
                [6847] = "TotalNumOfParts",
                [6849] = "PartNum",
                [20086] = "NoOfDealers",
                [20074] = "CanRespond",
                [20075] = "CanQuote",
                [20081] = "QuoteTimePeriod",
                [20090] = "DueinTimePeriod",
                [5745] = "MultipleTickets",
                [20000] = "FloatRateDayCount",
                [5722] = "FixedRatePayFrequency",
                [5775] = "FixedRateDayCount",
                [20095] = "OriginalPriceType",
                [20096] = "OrigianlPrice",
                [20097] = "DeltaRisk",
                [20073] = "NegotiationType",
                [20076] = "CanRequote",
                [22646] = "AdjustedStartDate",
                [20212] = "OnSEF",
                [22647] = "TwCalcNPV",
                [22552] = "PositionNettingTrade",
                [22510] = "FixedRateCALCFQ",
                [22512] = "FixedRatePAYRELTO",
                [22516] = "FixedRatePDTCTR",
                [22520] = "FixedRateCDTCTR",
                [22514] = "FixedRateADJDTPD",
                [22518] = "FixedRateADJDTCP",
                [22522] = "FloatRateCALCFQ",
                [22524] = "FloatRatePAYRELTO",
                [22526] = "FloatRateADJDTPD",
                [22528] = "FloatRatePDTCTR",
                [22532] = "FloatRateADJDTCP",
                [22534] = "FloatRateCDTCTR",
                [22536] = "FloatRateRESFREQ",
                [22538] = "FloatRateRESRELTO",
                [22540] = "FloatRateADJDTRES",
                [22542] = "FloatRateRDTCTR",
                [22544] = "FloatRateRESDAYS",
                [22546] = "FloatRateFDTYP",
                [22548] = "FloatRateFXDCONV",
                [22550] = "FloatRateFDTCTR",
                [523] = "PartySubID",
                [803] = "PartySubIDType",
            };

            #region Trade Values

            //9 BodyLength
            //35 MsgType
            //34 MsgSeqNum
            //49 SenderCompID
            //52 SendingTime
            //56 TargetCompID
            //58 Text
            //131 QuoteReqID
            //146 NoRelatedSym
            //55 Symbol
            //48 SecurityID
            //22 SecurityIDSource
            //460 Product
            //167 SecurityType
            //762 SecuritySubType
            //541 MaturityDate
            //1184 SecurityXMLLen
            //1185 SecurityXML
            //1186 SecurityXMLSchema
            //54 Side
            //38 OrderQty
            //64 SettlDate
            //15 Currency
            //60 TransactTime
            //221 BenchmarkCurveName
            //222 BenchmarkCurvePoint
            //423 PriceType
            //66 ListID
            //75 TradeDate
            //828 TrdType
            //10 CheckSum
            //453 NoPartyIDs
            //448 PartyID
            //447 PartyIDSource
            //452 PartyRole
            //802 NoPartySubIDs
            //523 PartySubID
            //803 PartySubIDType
            //20077,BookName
            //20078,TraderList
            //20079,TimeoutPeriod
            //22001,NettingTradeID
            //22005,CertaintyOfClearing
            //20184,ClearingCategory
            //5023,TickSize
            //6847,TotalNumOfParts
            //6849, PartNum
            //20086, NoOfDealers
            //20074, CanRespond
            //20075,CanQuote
            //20081,QuoteTimePeriod
            //20090,DueinTimePeriod
            //5745,MultipleTickets
            //20000,FloatRateDayCount
            //5722,FixedRatePayFrequency
            //5775,FixedRateDayCount
            //20095,OriginalPriceType
            //20096,OrigianlPrice
            //20097,DeltaRisk
            //20073,NegotiationType
            //20076,CanRequote
            //22646,AdjustedStartDate
            //20212,OnSEF
            //22647,TwCalcNPV
            //22552,PositionNettingTrade
            //22510,FixedRateCALCFQ
            //22512,FixedRatePAYRELTO
            //22516,FixedRatePDTCTR
            //22520,FixedRateCDTCTR
            //22514,FixedRateADJDTPD
            //22518,FixedRateADJDTCP
            //22522,FloatRateCALCFQ
            //22524,FloatRatePAYRELTO
            //22526,FloatRateADJDTPD
            //22528,FloatRatePDTCTR
            //22532,FloatRateADJDTCP
            //22534,FloatRateCDTCTR
            //22536,FloatRateRESFREQ
            //22538,FloatRateRESRELTO
            //22540,FloatRateADJDTRES
            //22542,FloatRateRDTCTR
            //22544,FloatRateRESDAYS
            //22546,FloatRateFDTYP
            //22548,FloatRateFXDCONV
            //22550,FloatRateFDTCTR
            //523,PartySubID
            //803,PartySubIDType

            #endregion

            return tradeValues;

        }
    }
}
