using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

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

        public static void AddPagination(this HttpResponse response,int currentPage,
        int itemsPerPage,int totalItems,int totalPages )
        {
            var paginationNumber = new PaginationHeader(currentPage,itemsPerPage,totalItems,totalPages);
            var camelCase = new JsonSerializerSettings();
            camelCase.ContractResolver = new CamelCasePropertyNamesContractResolver();
            response.Headers.Add("Pagination",JsonConvert.SerializeObject(paginationNumber,camelCase));
            response.Headers.Add("Access-Control-Expose-Headers","Pagination");
        }
        public static int CalculateAge(this DateTime date){
            var age = DateTime.Today.Year - date.Year;
             if (date.AddYears(age)> DateTime.Today)
                     age--;
            
            return age;
        }
    }
}