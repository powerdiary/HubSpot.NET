using System;
using System.Runtime.Serialization;

namespace HubSpot.NET.Api.LineItem.DTO
{
    [DataContract]
    public class LineItemPropertiesHubSpotModel
    {
        /// <summary>
        /// The name of the Line Item.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The price of the Line Item.
        /// </summary>
        [DataMember(Name = "price")]
        public decimal? Price { get; set; }

        /// <summary>
        /// The amount for the Line Item.
        /// </summary>
        [DataMember(Name = "amount")]
        public decimal? Amount { get; set; }

        /// <summary>
        /// The date the line item was created.
        /// </summary>
        [DataMember(Name = "createdate")]
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// ACV (Annual Contract Value) for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_acv")]
        public decimal? HsAcv { get; set; }

        /// <summary>
        /// ARR (Annual Recurring Revenue) for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_arr")]
        public decimal? HsArr { get; set; }

        /// <summary>
        /// MRR (Monthly Recurring Revenue) for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_mrr")]
        public decimal? HsMrr { get; set; }

        /// <summary>
        /// Margin for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_margin")]
        public decimal? HsMargin { get; set; }

        /// <summary>
        /// Total Contract Value (TCV) for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_tcv")]
        public decimal? HsTcv { get; set; }

        /// <summary>
        /// Post-tax amount for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_post_tax_amount")]
        public decimal? HsPostTaxAmount { get; set; }

        /// <summary>
        /// Pre-discount amount for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_pre_discount_amount")]
        public decimal? HsPreDiscountAmount { get; set; }

        /// <summary>
        /// Total discount for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_total_discount")]
        public decimal? HsTotalDiscount { get; set; }

        /// <summary>
        /// Recurring billing number of payments for the Line Item.
        /// </summary>
        [DataMember(Name = "hs_recurring_billing_number_of_payments")]
        public int? HsRecurringBillingNumberOfPayments { get; set; }
    }
}