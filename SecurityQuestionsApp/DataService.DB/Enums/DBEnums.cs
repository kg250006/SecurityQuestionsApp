using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#pragma warning disable 1591 // Disable "CS1591" missing XML comment warnings

namespace DBService.DB.Enums
{
    public class DBEnums
    {
        public enum Process_Stage
        {
            [System.Runtime.Serialization.EnumMember(Value = @"UNKNOWN")]
            UNKNOWN = -1,

            [System.Runtime.Serialization.EnumMember(Value = @"NEW_fromMWAPI")]
            NEW_fromMWAPI = 4,

            [System.Runtime.Serialization.EnumMember(Value = @"UPDATED_fromMWAPI")]
            UPDATED_fromMWAPI,

            [System.Runtime.Serialization.EnumMember(Value = @"SENT_toDTAPI")]
            SENT_toDTAPI,

            [System.Runtime.Serialization.EnumMember(Value = @"UPDATED_fromDTAPI")]
            UPDATED_fromDTAPI,

            [System.Runtime.Serialization.EnumMember(Value = @"FAILED_toDTAPI")]
            FAILED_toDTAPI,

            [System.Runtime.Serialization.EnumMember(Value = @"SENT_toPrinter")]
            SENT_toPrinter,

            [System.Runtime.Serialization.EnumMember(Value = @"IncompleteORDER")]
            IncompleteORDER,

            [System.Runtime.Serialization.EnumMember(Value = @"FAILED_Processing")]
            FAILED_Processing,

            [System.Runtime.Serialization.EnumMember(Value = @"FAILED_toPrint")]
            FAILED_toPrint,

            [System.Runtime.Serialization.EnumMember(Value = @"NEW_fromCHKMGR")]
            NEW_fromCHKMGR,

            [System.Runtime.Serialization.EnumMember(Value = @"SENT_toMWAPI")]
            SENT_toMWAPI,

            [System.Runtime.Serialization.EnumMember(Value = @"FAILED_toMWAPI")]
            FAILED_toMWAPI
        }

    }
}
