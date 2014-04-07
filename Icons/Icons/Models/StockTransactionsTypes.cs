using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public enum StockTransactionsTypes
    {
        شراء = 1,
        بيع = 2,
        امر_عمل = 3,
        ضبط_مخزن = 4,
        تعديل_فاتورة_شراء = 5,
        تعديل_فاتورة_بيع = 6,
        حذف_فاتورة_بيع = 7,
        حذف_فاتورة_شراء = 8
    }
}