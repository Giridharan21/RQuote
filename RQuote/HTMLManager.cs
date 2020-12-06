using HtmlAgilityPack;
using System;
using System.IO;

namespace RQuote
{
    public static class HTMLManager
    {
        public static string GetHTMLFor(QuotationPageDataContext quoteDetails)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(Utils.TemplateFilePath);
            //rq_firstName
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_firstName']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_firstName']").InnerHtml = quoteDetails.CustomerDetails.FirstName + " " + quoteDetails.CustomerDetails.LastName : "";
            //rq_lastName
            //_ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_lastName']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_lastName']").InnerHtml = quoteDetails.CustomerDetails.LastName : "";
            //rq_email
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_email']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_email']").InnerHtml = quoteDetails.CustomerDetails.Email : "";
            //rq_phNo
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_phNo']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_phNo']").InnerHtml = quoteDetails.CustomerDetails.Phone : "";
            //rq_orgName
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_orgName']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_orgName']").InnerHtml = quoteDetails.CustomerDetails.OrganizationName : "";
            //rq_projName
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_projName']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_projName']").InnerHtml = quoteDetails.CustomerDetails.ProjectName : "";
            //rq_quotNo
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_quotNo']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_quotNo']").InnerHtml = quoteDetails.QuotationNumber : "";
            //rq_quotDate
            _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_quotDate']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_quotDate']").InnerHtml = quoteDetails.QuotationDate : "";
            //rq_tableBody
            var tableBody = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_tableBody']");
            if(tableBody != null)
            {
                int index = 1;
                double total = 0;
                double totalAmount = 0;
                double totalGst = 0;
                foreach (var lineItem in quoteDetails.QuoteLines)
                {
                    tableBody.AppendChild(GetQuoteLineNode(lineItem, index++));
                    total += lineItem.Total;
                    totalAmount += lineItem.Amount;
                    totalGst += lineItem.GSTAmount;
                }
                //tableBody.AppendChild(GetQuoteLineNode(null, -1, total, totalGst, totalAmount));
                //rq_tableFooter
                //rq_amount_total
                _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_amount_total']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_amount_total']").InnerHtml = totalAmount.ToString() : "";
                //rq_gst_total
                _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_gst_total']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_gst_total']").InnerHtml = totalGst.ToString() : "";
                //rq_total
                _ = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_total']") != null ? htmlDoc.DocumentNode.SelectSingleNode("//*[@id='rq_total']").InnerHtml = total.ToString() : "";
            }

            return htmlDoc.DocumentNode.OuterHtml;
        }

        private static HtmlNode GetQuoteLineNode(QuoteLineItem lineItem, int index, double allTotal = -1, double gstTotal = -1, double amountTotal = -1)
        {
            string slNo = allTotal == - 1 ? index.ToString() : "";
            var partNo = allTotal == -1 ? lineItem.PartNo : "";
            var imgPath = allTotal == -1 && File.Exists(lineItem.Image) ? "data:image/jpeg;base64," + Convert.ToBase64String(File.ReadAllBytes(lineItem.Image)) : "";//lineItem.Image;
            var description = allTotal == -1 ? lineItem.ProductDetails.Replace("\n", "<br/>") : "";
            var unit = allTotal == -1 ? lineItem.Unit : "";
            var qty = allTotal == -1 ? lineItem.Quantity.ToString() : "";
            var price = allTotal == -1 ? lineItem.Price.ToString() : "";
            var discount = allTotal == -1 ? lineItem.Discount.ToString() + "%": "";
            var amount = allTotal == -1 ? lineItem.Amount.ToString() : amountTotal.ToString();
            var gst = allTotal == -1 ? lineItem.SelectedGST.ToString() + "%" : "";
            var gstAmt = allTotal == -1 ? lineItem.GSTAmount.ToString() : gstTotal.ToString();
            var total = allTotal == -1 ? lineItem.Total.ToString() : allTotal.ToString();
            var lineItemTemplate = $"<tr><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{slNo}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;max-width:80px;\"><p>{partNo}</p></div></td><td style=\"padding-top: 2px; padding-bottom: 2px;\"><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><img class=\"aligncenter\" style=\"width: 90px;margin-top: 0px;margin-bottom: 0px;\" onerror=\"this.style.display = 'none'\" alt=\"\" src=\"{imgPath}\"/></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: left;max-width:110px;\"><p>{description}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;max-width:60px;\"><p>{unit}</p></div> </td> <td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align:center;\"><p>{qty}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{price}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{discount}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{gst}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{amount}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{gstAmt}</p></div></td><td><div class=\"td-content-wrapper\"><div class=\"td-content\" style=\"text-align: center;\"><p>{total}</p></div></td></tr>";
            return HtmlNode.CreateNode(lineItemTemplate);
        }
    }
}
