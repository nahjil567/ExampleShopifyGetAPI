using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Threading;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {

            Dictionary<string, int> resultItems = new Dictionary<string, int>();
            Dictionary<string,string> result = new Dictionary<string, string>();
            CountRootObject countObj = JsonConvert.DeserializeObject<CountRootObject>(GetCount());
            int pages = countObj.count / 250 + 1;
            for (int curPage = 0; curPage < pages; curPage++)
            {
                RootObject dynObj = JsonConvert.DeserializeObject<RootObject>(GetOrders(curPage));

                foreach (Order o in dynObj.orders)
                {
                    Thread.Sleep(500);
                    TransactionRootObject tranObj = JsonConvert.DeserializeObject<TransactionRootObject>(GetTransactions(o.id));
                    o.transactions = tranObj.transactions;
                    if (o.transactions != null)
                    {
                        foreach (Transaction t in o.transactions)
                            if (t.status.Equals("success") && o.gateway.Equals("paypal") && !result.ContainsKey(t.authorization))
                            {
                                foreach (Fulfillment f in o.fulfillments)
                                {
                                    if (f.tracking_number != null)
                                    {
                                        result.Add(t.authorization,f.tracking_number);
                                    }
                                }
                            }
                    }
                }



            }
            System.IO.StreamWriter file = new System.IO.StreamWriter("d:\\result2.txt", true);

            foreach (KeyValuePair<string, string> entry in result)
            {
                file.WriteLine(entry.Key + "," + entry.Value);
            }
            file.Close();
        }
        public static string GetTransactions(Int64 orderId)
        {

            string url = "https://moon-beam.myshopify.com/admin/orders/"+orderId+"/transactions.json";
         
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/json";
            req.Credentials = GetCredential(url);
            req.PreAuthenticate = true;

            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    
                    string message = String.Format("Call failed. Received HTTP {0}", resp.StatusCode);
                    throw new ApplicationException(message);
                }

                var sr = new StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd();
            }
        }

        public static string GetCount()
        {

            const string url = "https://moon-beam.myshopify.com/admin/orders/count.json";

            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/json";
            req.Credentials = GetCredential(url);
            req.PreAuthenticate = true;

            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("Call failed. Received HTTP {0}", resp.StatusCode);
                    throw new ApplicationException(message);
                }

                var sr = new StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd();
            }
        }

        public static string GetOrders(int page)
        {
 
            string url = "https://moon-beam.myshopify.com/admin/orders.json?limit=250&page="+ page;
            
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            req.ContentType = "application/json";
            req.Credentials = GetCredential(url);
            req.PreAuthenticate = true;

            using (var resp = (HttpWebResponse)req.GetResponse())
            {
                if (resp.StatusCode != HttpStatusCode.OK)
                {
                    string message = String.Format("Call failed. Received HTTP {0}", resp.StatusCode);
                    throw new ApplicationException(message);
                }

                var sr = new StreamReader(resp.GetResponseStream());
                return sr.ReadToEnd();
            }
        }

        private static CredentialCache GetCredential(string url)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            var credentialCache = new CredentialCache();
            credentialCache.Add(new Uri(url), "Basic", new NetworkCredential("", ""));
            return credentialCache;
        }

    }
 
 



 




 

    public class Fulfillment
    {
        public Int64 id { get; set; }
        public Int64 order_id { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string service { get; set; }
        public string updated_at { get; set; }
        public object tracking_company { get; set; }
        public string tracking_number { get; set; }
        public List<string> tracking_numbers { get; set; }
        public string tracking_url { get; set; }
        public List<string> tracking_urls { get; set; }

    }



   




    public class Transaction
    {
        public Int64 id { get; set; }
        public Int64 order_id { get; set; }
        public string amount { get; set; }
        public string kind { get; set; }
        public string gateway { get; set; }
        public string status { get; set; }
        public object message { get; set; }
        public string created_at { get; set; }
        public bool test { get; set; }
        public string authorization { get; set; }
        public string currency { get; set; }
        public object location_id { get; set; }
        public object user_id { get; set; }
        public object parent_id { get; set; }
        public object device_id { get; set; }
        public object error_code { get; set; }
        public string source_name { get; set; }
    }



    public class Customer
    {
        public Int64 id { get; set; }
        public string email { get; set; }
        public bool accepts_marketing { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public Int64 orders_count { get; set; }
        public string state { get; set; }
        public string total_spent { get; set; }
        public Int64 last_order_id { get; set; }
        public object note { get; set; }
        public bool verified_email { get; set; }
        public object multipass_identifier { get; set; }
        public bool tax_exempt { get; set; }
        public string tags { get; set; }
    }

    public class Order
    {
        public Int64 id { get; set; }
        public string email { get; set; }
        public object closed_at { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
        public Int64 number { get; set; }
        public object note { get; set; }
        public string token { get; set; }
        public string gateway { get; set; }
        public bool test { get; set; }
        public string total_price { get; set; }
        public string subtotal_price { get; set; }
        public int total_weight { get; set; }
        public string total_tax { get; set; }
        public bool taxes_included { get; set; }
        public string currency { get; set; }
        public string financial_status { get; set; }
        public bool confirmed { get; set; }
        public string total_discounts { get; set; }
        public string total_line_items_price { get; set; }
        public string cart_token { get; set; }
        public bool buyer_accepts_marketing { get; set; }
        public string name { get; set; }
        public string referring_site { get; set; }
        public string landing_site { get; set; }
        public object cancelled_at { get; set; }
        public object cancel_reason { get; set; }
        public string total_price_usd { get; set; }
        public object checkout_token { get; set; }
        public string reference { get; set; }
        public object user_id { get; set; }
        public object location_id { get; set; }
        public string source_identifier { get; set; }
        public object source_url { get; set; }
        public string processed_at { get; set; }
        public object device_id { get; set; }
        public object browser_ip { get; set; }
        public string landing_site_ref { get; set; }
        public Int64 order_number { get; set; }
        public List<string> payment_gateway_names { get; set; }
        public string processing_method { get; set; }
        public Int64 checkout_id { get; set; }
        public string source_name { get; set; }
        public object fulfillment_status { get; set; }
        public string tags { get; set; }
        public string contact_email { get; set; }
        public object order_status_url { get; set; }

        public List<Fulfillment> fulfillments { get; set; }
        public List<Transaction> transactions { get; set; }

        public Customer customer { get; set; }
    }

    public class RootObject
    {
        public List<Order> orders { get; set; }
    }
    public class TransactionRootObject
    {
        public List<Transaction> transactions { get; set; }
    }
    public class FulfillmentRootObject
    {
        public List<Fulfillment> fulfillments { get; set; }
    }

    public class CountRootObject
    {
        public int count { get; set; }
    }
}  

