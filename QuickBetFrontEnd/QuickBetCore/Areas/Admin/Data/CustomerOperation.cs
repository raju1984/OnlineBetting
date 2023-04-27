using Microsoft.EntityFrameworkCore;
using QuickBetCore.DatabaseEntity;
using QuickBetCore.Models;
using QuickBetCore.Resource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QuickBetCore.Areas.Admin.Data
{

    public class ChanegPassowrdViewModel
    {
        public int UserId { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
    public class IdViewModel
    {
        public int Id { get; set; }
    }
    public class UpdateModel
    {
        public int Id { get; set; }
        public decimal value { get; set; }
        public string type { get; set; }
    }
    public class BlockUnblockModel
    {
        public string type { get; set; }
        public int Id { get; set; }
    }
    public class CustomerOperation
    {
        public static List<UserListViewModel> GetAgentCustomers(int AgentId)
        {
            List<UserListViewModel> userLists = new List<UserListViewModel>();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var agentCustomers = db.AgentCustomers.Include("User").Where(a => a.AgentId == AgentId).ToList();
                    userLists = (from r in agentCustomers
                                 select new UserListViewModel
                                 {
                                     Id = r.Id,
                                     name = r.User.Name,
                                     email = r.User.Email,
                                     countrycode = r.User.CountryCode,
                                     phone = r.User.ContactNo,
                                     status = r.User.UserStatus,
                                     profile = ApplicatiopnCommonFunction.GetImage(r.User.ProfilePicture, ImageType.dp.ToString()),
                                     walletbalance = r.User.MyWalletbalance
                                 }).ToList();
                }
            }
            catch (Exception ex)
            {

            }
            return userLists;
        }
        public static CustomerUploadModel GetCustomer(int customerId, int userType)
        {
            CustomerUploadModel customer = new CustomerUploadModel();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    int userStatus = (int)UserStatus.deleted;
                    var data = db.Users.Where(x => x.Id == customerId && x.UserType == userType && x.UserStatus != userStatus).FirstOrDefault();
                    if (data != null)
                    {
                        customer.Id = data.Id;
                        customer.Name = data.Name;
                        customer.DisplayName = data.DisplayName;
                        customer.Email = data.Email;
                        customer.Password = data.Password;
                        customer.ProfilePicture = data.ProfilePicture;
                        customer.ContactNo = data.ContactNo;
                    }
                }
            }
            catch { }
            return customer;
        }

        public static ApiResponse CreateUpdateCustomer(CustomerUploadModel model, int UserType)
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
                            customer.AgentCommison = Convert.ToDecimal(ApplicationVariable.defaultSuperAgentCommission);

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
                        _user.UserStatus = (int)UserStatus.active;
                        _user.Password = model.Password;
                        _user.CreatedAt = DateTime.Now;
                        _user.UpdatedAt = DateTime.Now;
                        db.Users.Add(_user);
                        db.SaveChanges();
                        string SincerelyName = "QuickBet Team";
                        String Subject = "Welcome mail";
                        var MailUserType = "";
                        if (UserType == 3)
                        {
                            MailUserType = "Agent";
                        }
                        else if (UserType == 5)
                        {
                            MailUserType = "Super Agent";
                        }
                        else if (UserType == 6)
                        {
                            MailUserType = "Mobile Agent";
                        }
                        String Body = "Hi " + _user.Name + "<br><br>" + "Welcome to quickbet games, Your account has been successfully generated as" + MailUserType + ". Use the below mentioned link for login" + "<br><br>" + "https://quickbetng.com/Account/Login" +
                        "<br><br>Sincerely,<br>" + SincerelyName;
                        CommonFunction.SendEmail(_user.Email, _user.Name, 1, Subject, Body);

                        response.Code = (int)ApiResponseCode.ok;
                        response.Msg = "Registered Successfully..";
                    }
                }
            }
            catch { response.Code = (int)ApiResponseCode.fail; response.Msg = "Failed to process.."; }
            return response;
        }

        public static ApiResponse DeleteUser(int Id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
            {

                try
                {
                    var users = dbConn.Users.Where(a => a.Id == Id).FirstOrDefault();
                    // users.Password = "123456";
                    users.UserStatus = (int)UserStatus.deleted;
                    dbConn.SaveChanges();
                    resp.Code = (int)ApiResponseCode.ok;
                    resp.Msg = "Deleted Sucessfully!!";

                }
                catch (Exception ex)
                {
                    resp.Msg = ex.Message;
                }
                return resp;
            }
        }

        public static ApiResponse DeleteUserSuperAgent(int Id, int AdminUserId)
        {
            dashboard resp = new dashboard();
            resp.Code = (int)ApiResponseCode.fail;
            using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
            {

                try
                {
                    var users = dbConn.Users.Where(a => a.Id == Id).FirstOrDefault();
                    // users.Password = "123456";
                    users.UserStatus = (int)UserStatus.deleted;

                    dbConn.SaveChanges();
                    resp.Code = (int)ApiResponseCode.ok;
                    resp.Msg = "Deleted Sucessfully!!";
                    resp.BlockUser = dbConn.Users.Where(a => a.UserType == (int)UserType.Agent && a.ParentAgentId == AdminUserId && a.UserStatus == (int)UserStatus.block).Count();
                    resp.ActiveUser = dbConn.Users.Where(a => a.UserType == (int)UserType.Agent && a.ParentAgentId == AdminUserId && a.UserStatus == (int)UserStatus.active).Count();
                }
                catch (Exception ex)
                {
                    resp.Msg = ex.Message;
                }
                return resp;
            }
        }

        public static ApiResponse ResetUserPassword(int Id)
        {
            ApiResponse resp = new ApiResponse();
            resp.Code = (int)ApiResponseCode.fail;
            using (QuickbetDbEntities dbConn = new QuickbetDbEntities())
            {

                try
                {
                    var Newpassword = ApplicatiopnCommonFunction.GetRandomPassword(6);
                    var users = dbConn.Users.Where(a => a.Id == Id).FirstOrDefault();
                    users.Password = Newpassword;
                    dbConn.SaveChanges();
                    resp.Code = (int)ApiResponseCode.ok;
                    resp.Msg = "Password has been reset Sucessfully!, New password is " + Newpassword + "";

                }
                catch (Exception ex)
                {
                    resp.Msg = ex.Message;
                }
                return resp;
            }
        }

        public static ApiResponse TransferFund(FundTransferModel model, int AdminUserId)
        {
            ApiResponse response = new ApiResponse();
            try
            {
                using (QuickbetDbEntities db = new QuickbetDbEntities())
                {
                    var objWalletTransactions = db.WalletTransactions.Where(a => a.UserId == model.UserId)
                        .OrderByDescending(a => a.InsertDate).FirstOrDefault();
                    decimal ClosingBalance = 0;
                    if (objWalletTransactions != null)
                    {
                        ClosingBalance = objWalletTransactions.ClosingBalance;
                    }
                    WalletTransaction obj = new WalletTransaction();
                    obj.UserId = model.UserId;
                    obj.TransferType = model.TypeOfTransfer == (int)TransType.Debit ? (int)WalletTransactionType.Deducted_by_Admin : (int)WalletTransactionType.Credited_by_Admin;
                    obj.Status = (int)WalletTransactionStatusType.TransactionSuccess;
                    obj.TransType = model.TypeOfTransfer;
                    obj.ClosingBalance = model.TypeOfTransfer == (int)TransType.Credit ? ClosingBalance + Convert.ToDecimal(model.Amount) : ClosingBalance - Convert.ToDecimal(model.Amount);
                    obj.Amount = Convert.ToDecimal(model.Amount);
                    obj.Note = model.Description;
                    obj.TransactionRemark = "Amount:₦" + obj.Amount + " Added to your wallet successfully by Admin (" + AdminUserId + ")";
                    obj.InsertDate = DateTime.UtcNow;
                    obj.LastUpdated = DateTime.UtcNow;
                    var objuser = db.Users.Where(a => a.Id == model.UserId).FirstOrDefault();
                    objuser.MyWalletbalance = obj.ClosingBalance;
                    db.WalletTransactions.Add(obj);
                    if (db.SaveChanges() > 0)
                    {
                        //string SincerelyName = "QuickBet Team";
                        //String Subject = "Credit amount by admin";

                        //String Body = "Hi " + objuser.Name + ",<br><br>Amount:₦ " +
                        //obj.Amount + "Added to your wallet successfully by Admin" + ApplicationSession.AdminSession.Id +
                        //"<br><br>Sincerely,<br>" + SincerelyName;
                        //CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);
                        if (model.TypeOfTransfer == 0)
                        {
                            string SincerelyName = "QuickBet Team";
                            String Subject = " Fund Trasfer";

                            String Body = " Hi " + objuser.Name + " ,<br><br>Congratulations " +
                            " Amount:₦" + obj.Amount + " Has Been debited In Account Of by Admin." +
                            "<br><br>Sincerely,<br>" + SincerelyName;
                            CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);
                        }
                        else
                        {
                            string SincerelyName = "QuickBet Team";
                            String Subject = " Fund Trasfer";

                            String Body = " Hi " + objuser.Name + " ,<br><br>Congratulations " +
                            " Amount of " + obj.Amount + " Has Been Credited in Account by Admin. " +
                            "<br><br>Sincerely,<br>" + SincerelyName;
                            CommonFunction.SendEmail(objuser.Email, objuser.Name, 1, Subject, Body);
                        }

                        response.txnId = obj.Id.ToString();
                        response.Code = (int)ApiResponseCode.ok; response.Msg = "Transaction Processed successfully for " + objuser.Email + ".";

                    }
                }
            }
            catch
            {
                response.Code = (int)ApiResponseCode.fail; response.Msg = "Failed to process..";
            }

            return response;
        }
    }
}
