using System;
using System.Collections.Generic;
using System.Text;

namespace RateLimiter.Service.Util
{
    public class DEPARTMENT
    {
        public const string INSERT_MESSAGE = "The department has been added to DATABASE";
        public const string DELETE_MESSAGE = "The deparment has been deleted from DATABASE";
        public const string LIST_MESSAGE = "The department list has been retrivied from DATABASE";
        public const string DOES_NOT_EXIST = "The department does not exists in the DATABASE";
    }

    public class EMPLOYEE
    {
        public const string INSERT_MESSAGE = "The employee has been added to DATABASE";
        public const string DELETE_MESSAGE = "The employee has been deleted from DATABASE";
        public const string LIST_MESSAGE = "The employee list has been retrivied from DATABASE";
        public const string DOES_NOT_EXIST = "The employee does not exists in the DATABASE";
    }
}
