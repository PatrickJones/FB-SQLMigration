using NuLibrary.Migration.Mappings.InMemoryMappings;
using NuLibrary.Migration.SQLDatabase.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuLibrary.Migration.SQLDatabase.SQLHelpers
{
    public class SubscriptionHandler
    {
        NumedicsGlobalHelpers nh;

        public Guid UserId { get; private set; }

        public clinipro_Users cliniProUser { get; set; }
        public ICollection<subs_CheckPayments> CheckPayments = new List<subs_CheckPayments>();
        public ICollection<subs_Adjustments> Adjustments = new List<subs_Adjustments>();
        public ICollection<subs_PayPalPayments> PayPalPayments = new List<subs_PayPalPayments>();
        public ICollection<subs_Trials> Trials = new List<subs_Trials>();
        public ICollection<subs_Subscriptions> Subscriptions = new List<subs_Subscriptions>();

        private ICollection<Subscription> subscriptions = new List<Subscription>();
        private Guid applicationId = Guid.Parse("5E1A0790-68AA-405D-908A-4AB578832EFE");
        private Guid institutionId = Guid.Empty;
        private int UserType = 2;

        public SubscriptionHandler(Guid userId)
        {
            UserId = userId;

            nh = new NumedicsGlobalHelpers();
            institutionId = MemoryMappings.GetAllInstitutions().Where(w => w.LegacySiteId == 20001).Select(s => s.InstitutionId).FirstOrDefault();// nh.GetInstitutionId(20001); // TODO: place in config file, because will change based on environment
        }

        public ICollection<Subscription> GetMappedSubscriptions()
        {
            MapCheckPayments();
            MapAdjustments();
            MapPayPalPayments();
            MapTrials();

            return subscriptions;
        }

        private void MapTrials()
        {
            try
            {
                if (Trials.Count != 0)
                {
                    foreach (var tr in Trials)
                    {

                        var sub = new Subscription
                        {
                            ApplicationId = applicationId,
                            UserId = UserId,
                            UserType = UserType,
                            SubscriptionType = 0,
                            SubscriptionDate = tr.Date,
                            ExpirationDate = tr.Date.AddDays(tr.Days),
                            IsTrial = true,
                            InstitutionId = institutionId
                        };

                        subscriptions.Add(sub);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error mapping trials.", e);
            }
        }

        private void MapPayPalPayments()
        {
            try
            {
                if (PayPalPayments.Count != 0)
                {
                    foreach (var pp in PayPalPayments)
                    {
                        var nPayment = new Payment
                        {
                            PaymentMethod = 1,
                            PaymentApproved = true
                        };

                        var nPay = new PayPal()
                        {
                            mc_fee = pp.mc_fee,
                            mc_gross = pp.mc_gross,
                            parent_txn_id = pp.parent_txn_id,
                            payment_date = pp.payment_date,
                            payment_status = pp.payment_status,
                            PayPal_post_vars = pp.PayPal_post_vars,
                            pending_reason = pp.pending_reason,
                            reason_code = pp.reason_code,
                            SourceIP = pp.SourceIP,
                            txn_id = pp.txn_id
                        };

                        var sub = new Subscription
                        {
                            ApplicationId = applicationId,
                            UserId = UserId,
                            UserType = UserType,
                            SubscriptionType = GetSubscriptionType((byte)pp.MonthsPurchased),
                            SubscriptionDate = pp.Date,
                            ExpirationDate = pp.Date.AddMonths(pp.MonthsPurchased),
                            InstitutionId = institutionId

                        };

                        nPayment.PayPal = nPay;
                        sub.Payment = nPayment;

                        subscriptions.Add(sub);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error mapping PayPal payments.", e);
            }
        }

        private void MapAdjustments()
        {
            try
            {
                if (Adjustments.Count != 0)
                {
                    foreach (var ad in Adjustments)
                    {
                        var nPayment = new Payment
                        {
                            PaymentMethod = 4,
                            PaymentApproved = true
                        };

                        var sub = new Subscription
                        {
                            ApplicationId = applicationId,
                            UserId = UserId,
                            UserType = UserType,
                            SubscriptionType = 7,
                            SubscriptionDate = ad.Date,
                            ExpirationDate = (ad.SubscriptionTimeType.ToLower().StartsWith("day", StringComparison.Ordinal)) ? ad.Date.AddDays(ad.SubscriptionTimeAmount) : ad.Date.AddMonths(ad.SubscriptionTimeAmount),
                            InstitutionId = institutionId

                        };

                        sub.Payment = nPayment;
                        subscriptions.Add(sub);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error mapping adjustments.", e);
            }
        }

        private void MapCheckPayments()
        {
            try
            {
                if (CheckPayments.Count != 0)
                {
                    foreach (var cp in CheckPayments)
                    {
                        var nPayment = new Payment
                        {
                            PaymentMethod = 2,
                            PaymentApproved = (cp.Status.ToLower() == "completed") ? true : false
                        };

                        var nCheck = new Check
                        {
                            CheckStatus = GetCheckStatus(cp.Status),
                            CheckAmount = cp.Amount,
                            CheckCode = (cp.Code.HasValue) ? cp.Code.Value : 0,
                            CheckNumber = cp.CheckNumber,
                            CheckDateRecieved = cp.Date
                        };

                        var sub = new Subscription
                        {
                            ApplicationId = applicationId,
                            UserId = UserId,
                            UserType = UserType,
                            SubscriptionType = GetSubscriptionType(cp.MonthsPurchased),
                            SubscriptionDate = cp.Date,
                            ExpirationDate = cp.Date.AddMonths(cp.MonthsPurchased),
                            InstitutionId = institutionId

                        };

                        nPayment.Check = nCheck;
                        sub.Payment = nPayment;

                        subscriptions.Add(sub);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error mapping check payments", e);
            }
        }

        private int GetSubscriptionType(byte monthsPurchased)
        {
            switch (monthsPurchased)
            {
                case 1:
                    return 1;
                case 3:
                    return 2;
                case 6:
                    return 3;
                case 12:
                    return 4;
                default:
                    return 1;
            }
        }

        private int GetCheckStatus(string status)
        {
            switch (status.ToLower())
            {
                case "completed":
                    return 1;
                case "canceled":
                    return 2;
                case "pending":
                    return 3;
                case "rejectedbybank":
                    return 4;
                default:
                    return 1;
            }
        }
    }
}
