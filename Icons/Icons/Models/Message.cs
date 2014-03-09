using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Icons.Models
{
    public enum Message
    {
        Success_Login = 0,
        Username_or_password_is_wrong = 1,
        Username_Already_Exists = 2,
        Account_created_successfully = 3,
        User_Deleted_Successfully = 4,
        Supplier_created_successfully = 5,
        Supplier_deleted_successfully = 6,
        User_updated_successfully = 7,
        Supplier_updated_successfully = 8,
        Project_Deleted_Successfully = 9,
        Project_Created_Successfully = 10,
        Project_Updated_Successfully = 11,
        Unit_Created_Successfully = 12,
        Unit_Delete_Successfully = 13,
        Unit_Updated_Successfully = 14,
        Product_Category_Created_Successfully = 15,
        Product_Category_Deleted_Successfully = 16,
        Product_Category_Updated_Successfully = 17,
        Product_Created_Successfully = 18,
        Product_Update_Successfully = 19,
        Product_Deleted_Successfully = 20,
        Product_Name_Already_Exist = 21,
        Invoice_Line_Removed_Successfully = 22,
        Invoice_Added_Successfully = 23,
        Customer_Name_Already_Exist = 24,
        Customer_Created_Successfully = 25,
        Customer_Updated_Succeessfully = 26,
        Customer_Deleted_Successfully = 27
    }
}