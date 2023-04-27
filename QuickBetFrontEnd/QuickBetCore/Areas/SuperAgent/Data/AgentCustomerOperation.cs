using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.SuperAgent.Data
{
    public class AgentCustomerOperation
    {
        public static ApiResponse CreateUpdateCustomer(CustomerUploadModel model, int UserType,int UserId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    string email = model.Email.Trim().ToLower();
                    string ContactNo = model.ContactNo.Trim().Replace(" ", String.Empty).ToLower();
                    if (model.Id > 0)
                    {
                        int status = (int)UserStatus.deleted;
                        var isExistEmail = db.Users.Any(x => x.Id != model.Id && x.Email == email);
                        if (isExistEmail)
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = Applicationstring.StringUserExistEmailMesssage;
                            return response;
                        }
                        var isExistPhone = db.Users.Any(x => x.Id != model.Id && x.ContactNo == ContactNo);
                        if (isExistPhone)
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = Applicationstring.StringUserExistPhoneMesssage;
                            return response;
                        }

                        var customer = db.Users.Where(x => x.Id == model.Id && x.UserStatus != status).FirstOrDefault();
                        if (customer != null)
                        {
                            customer.Name = model.Name;
                            if (!string.IsNullOrEmpty(model.DisplayName))
                            {
                                customer.DisplayName = model.DisplayName;
                            }
                            customer.Email = model.Email;
                            customer.Password = model.Password;
                            customer.ContactNo = model.ContactNo;
                            customer.CountryCode = model.CountryCode;
                            customer.ParentAgentId = UserId;
                            customer.UpdatedAt = DateTime.UtcNow;
                            if (!string.IsNullOrEmpty(model.ProfilePicture))
                            {
                                customer.ProfilePicture = model.ProfilePicture;
                            }
                            db.SaveChanges();
                            response.Code = (int)ApiResponseCode.ok;
                            response.Msg = "Updated Successfully..";
                        }
                        else { response.Code = (int)ApiResponseCode.fail; response.Msg = "Failed to process.."; }
                    }
                    else
                    {
                        var isExist = db.Users.Where(x => x.Email == email).FirstOrDefault();
                        if (isExist != null)
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = Applicationstring.StringUserExistEmailMesssage;
                            return response;
                        }
                        var isExistPhone = db.Users.Any(x => x.ContactNo == ContactNo);
                        if (isExistPhone)
                        {
                            response.Code = (int)ApiResponseCode.fail;
                            response.Msg = Applicationstring.StringUserExistPhoneMesssage;
                            return response;
                        }


                        QuickBetCore.DatabaseEntity.User _user = new QuickBetCore.DatabaseEntity.User();
                        _user.Name = model.Name;
                        if (!string.IsNullOrEmpty(model.DisplayName))
                        {
                            _user.DisplayName = model.DisplayName;
                        }
                        _user.Email = model.Email;
                        _user.ContactNo = model.ContactNo;
                        _user.CountryCode = model.CountryCode;
                        if (!string.IsNullOrEmpty(model.ProfilePicture))
                        {
                            _user.ProfilePicture = model.ProfilePicture;
                        }
                        _user.UserType = UserType;// (int)UserType.Users;
                        _user.UserStatus = (int)UserStatus.Pending_for_approval;
                        _user.Password = model.Password;
                        _user.CreatedAt = DateTime.Now;
                        _user.UpdatedAt = DateTime.Now;
                        _user.AgentCommison = Convert.ToDecimal(ApplicationVariable.defaultagentcommison);
                        _user.AgentCashBackOnPayment = Convert.ToDecimal(ApplicationVariable.defaultagentcashback);
                        _user.CustomerRetentionPeriod = ApplicationVariable.CustomerRetentionPeriod;
                        _user.ParentAgentId = UserId;
                        db.Users.Add(_user);
                        db.SaveChanges();
                        response.Code = (int)ApiResponseCode.ok;
                        response.Msg = "Registered Successfully..";
                    }
                }
            }
            catch { response.Code = (int)ApiResponseCode.fail; response.Msg = "Failed to process.."; }
            return response;
        }
    }
}
