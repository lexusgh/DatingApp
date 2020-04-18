using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace DatingApp.API.Helpers
{
    public static class Extensions
    {
        public static void AddErrorMessage(this HttpResponse response,string message)
        {
            response.Headers.Add("Application-Error",message);
            response.Headers.Add("Access-Control-Expose-Headers","Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin","*");

        }

        public static int CalculateAge(this DateTime date){
            var age = DateTime.Today.Year - date.Year;
             if (date.AddYears(age)> DateTime.Today)
                     age--;
            
            return age;
        }
    }
}