using EmployeeAPI.DATA.Database;
using EmployeeAPI.Model;
using EmployeeAPI.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;

namespace EmployeeAPI.Controllers
{

    public class EmployeeController : ApiController
    {
        [HttpGet]
        [ActionName("GetAllEmployees")]
        public HttpResponseMessage GetAllEmployees()
        {

            try
            {

                string query = "exec spEmployee  @q=0";
                var data = DataContext.GetDataContex().GetDataTableByQuery(query);
                if (data.DataSet.Tables[0].Rows.Count > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, data, RequestFormat.JsonFormaterString());
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new ConfirmationMsg { status = "Data Not Found", code = "404" }, RequestFormat.JsonFormaterString());
                }
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "error" + ex.Message.ToString(), code = "204" }, RequestFormat.JsonFormaterString());
            }

        }

        [HttpGet]
        [ActionName("GetAllEmployeeByID")]
        public HttpResponseMessage GetAllEmployeeByID(string eid)
        {

            try
            {

                string query = "exec spEmployee  @q=1,@Id='" + eid + "'";
                var data = DataContext.GetDataContex().GetDataTableByQuery(query);
                if (data.DataSet.Tables[0].Rows.Count>0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, data, RequestFormat.JsonFormaterString());
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, new ConfirmationMsg { status = "Data Not Found" , code = "404" }, RequestFormat.JsonFormaterString());
                }

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "error" + ex.Message.ToString(), code = "204" }, RequestFormat.JsonFormaterString());
            }

        }

        [HttpPost]
        [ActionName("InsertEmployee")]
        public HttpResponseMessage InsertEmployee(Employee objEMp)
        {
            try
            {
              
                string query = "exec spEmployee  @q=2,@Id='" + objEMp.Id + "',@FirstName='" + objEMp.FirstName + "',@MiddleName='" + objEMp.MiddleName + "',@LastName='" + objEMp.LastName + "'";
                var data = DataContext.GetDataContex().CRUDOperationsByQuery(query);
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "Employee Created Successfully with ID :" + objEMp.Id, code = "200" }, RequestFormat.JsonFormaterString());

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "error:" + ex.Message.ToString() + "", code = "204" }, RequestFormat.JsonFormaterString());
            }

        }


        [HttpPut]
        [ActionName("UpdateEmployee")]
        public HttpResponseMessage UpdateEmployee(Employee objEMp)
        {
            try
            {

                string query = "exec spEmployee  @q=2,@Id='" + objEMp.Id + "',@FirstName='" + objEMp.FirstName + "',@MiddleName='" + objEMp.MiddleName + "',@LastName='" + objEMp.LastName + "'";
                var data = DataContext.GetDataContex().CRUDOperationsByQuery(query);
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "Employee Update Successfully with ID :" + objEMp.Id, code = "200" }, RequestFormat.JsonFormaterString());

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "error:" + ex.Message.ToString() + "", code = "204" }, RequestFormat.JsonFormaterString());
            }

        }

        [HttpDelete]
        [ActionName("DeleteEmployee")]
        public HttpResponseMessage DeleteEmployee(Employee objEMp)
        {
            try
            {

                string query = "exec spEmployee  @q=3,@Id='" + objEMp.Id + "'";
                var data = DataContext.GetDataContex().CRUDOperationsByQuery(query);
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "Employee Deleted Successfully with ID :" + objEMp.Id, code = "200" }, RequestFormat.JsonFormaterString());

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ConfirmationMsg { status = "error:" + ex.Message.ToString() + "", code = "204" }, RequestFormat.JsonFormaterString());
            }

        }
    }
}