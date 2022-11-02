using System;
using System.Collections.Generic;
using System.Globalization;
using PirateKing.Constants;
using PirateKing.Core;
using PirateKing.Guards;
using PirateKing.Models;

namespace PirateKing.Tools
{
    public class ExpenseTools
    {
        /// <summary>
        /// Get <see cref="ExpenseCategory"/> from given transaction name & categories
        /// </summary>
        /// <param name="name"></param>
        /// <param name="categoryId"></param>
        /// <param name="categories"></param>
        /// <returns><see cref="ExpenseCategory"/></returns>
        public static ExpenseCategory GetExpenseCategory(
            string name, 
            string categoryId, 
            IList<string> categories)
        {
            Validate.NotNullOrEmpty(name, nameof(name));
            Validate.NotNullOrEmpty(categoryId, nameof(categoryId));
            Validate.NotNullOrEmpty(categories, nameof(categories));

            name = name.ToLower();

            if (name.Contains("donation") == true) return ExpenseCategory.Special;

            string category1 = categories[0];
            string category2 = (categories.Count > 1) ? categories[1] : null;
            string category3 = (categories.Count > 2) ? categories[2] : null;

            switch (category1)
            {
                case "Community":
                    return GetExpenseCategoryCommunity(name, categoryId, category2, category3);

                case "Food and Drink":
                    return GetExpenseCategoryFoodAndDrink(name, categoryId, category2, category3);

                case "Interest":
                    return GetExpenseCategoryInterest(name, categoryId, category2, category3);

                case "Payment":
                    return GetExpenseCategoryPayment(name, categoryId, category2, category3);

                case "Recreation":
                    return GetExpenseCategoryRecreation(name, categoryId, category2, category3);

                case "Service":
                    return GetExpenseCategoryService(name, categoryId, category2, category3);

                case "Shops":
                    return GetExpenseCategoryShops(name, categoryId, category2, category3);

                case "Tax":
                    return GetExpenseCategoryTax(name, categoryId, category2, category3);

                case "Transfer":
                    return GetExpenseCategoryTransfer(name, categoryId, category2, category3);

                case "Travel":
                    return GetExpenseCategoryTravel(name, categoryId, category2, category3);

                default:
                    return ExpenseCategory.Others;
            }
        }

        /// <summary>
        /// Get customized note for a given transaction 
        /// <see cref="ExpenseCategory"/>, name, and categories
        /// </summary>
        /// <param name="expenseCategory"></param>
        /// <param name="name"></param>
        /// <param name="categoryId"></param>
        /// <param name="categories"></param>
        /// <returns></returns>
        public static string GetNote(
            ExpenseCategory expenseCategory, 
            string name, 
            string categoryId,
            IList<string> categories)
        {
            name = name.ToLower();

            switch (expenseCategory)
            {
                case ExpenseCategory.Utility:
                    if (name.Contains("minol usa") == true) return "Water, Sewer, Storm & Trash";
                    if (name.Contains("vesta *at&t") == true) return "Telecommunication";
                    if (name.Contains("comcast") == true) return "Internet";
                    if (name.Contains("puget sound") == true) return "Electricity";

                    break;

                case ExpenseCategory.Vehicle:
                    if (name.Contains("geico") == true) return "Auto Insurance";
                    if (name.Contains("redmond tire pros") == true) return "Engine oil replacement";

                    break;

                case ExpenseCategory.Special:
                    if (name.Contains("edipayment") == true) return "Paycheck";
                    if (name.Contains("auto lease") == true) return "Auto Lease";
                    if (name.Contains("toyota financial") == true) return "Auto Lease";
                    if (name.Contains("microsoft co entr") == true) return "Microsoft Paycheck";

                    break;
            }

            return null;
        }

        private static ExpenseCategory GetExpenseCategoryCommunity(string name, string categoryId, string category2, string category3)
        {
            switch (category2)
            {
                case "Religious":
                    return ExpenseCategory.Special;
            }

            return ExpenseCategory.Others;
        }

        private static ExpenseCategory GetExpenseCategoryFoodAndDrink(string name, string categoryId, string category2, string category3)
        {
            if (name.Contains("good2go") == true) return ExpenseCategory.Vehicle;
            if (name.Contains("korean air") == true) return ExpenseCategory.Recreation;
            if (name.Contains("theory") == true) return ExpenseCategory.Shopping;
            if (name.Contains("crystal mountain") == true) return ExpenseCategory.Recreation;
            if (name.Contains("mint mobile") == true) return ExpenseCategory.Utility;
            if (name.Contains("sea-tac") == true) return ExpenseCategory.Vehicle;
            if (name.Contains("drinkpod") == true) return ExpenseCategory.Shopping;
            if (name.Contains("soundsonline") == true) return ExpenseCategory.PirateKing;

            return ExpenseCategory.Meal;
        }

        private static ExpenseCategory GetExpenseCategoryInterest(string name, string categoryId, string category2, string category3)
        {
            return ExpenseCategory.Special;
        }

        private static ExpenseCategory GetExpenseCategoryPayment(string name, string categoryId, string category2, string category3)
        {
            if (name.Contains("stevens pass") == true)
            {
                return ExpenseCategory.Recreation;
            }

            return ExpenseCategory.Special;
        }

        private static ExpenseCategory GetExpenseCategoryRecreation(string name, string categoryId, string category2, string category3)
        {
            return ExpenseCategory.Recreation;
        }

        private static ExpenseCategory GetExpenseCategoryService(string name, string categoryId, string category2, string category3)
        {
            if (name.Contains("wirebarley") == true) return ExpenseCategory.Special;
            if (name.Contains("hand & stone") == true) return ExpenseCategory.Recreation;
            if (name.Contains("netflix") == true) return ExpenseCategory.Recreation;
            if (name.Contains("zoom management") == true) return ExpenseCategory.Others;
            if (name.Contains("splice com") == true) return ExpenseCategory.PirateKing;

            switch (category2)
            {
                case "Financial":
                case "Business Services":
                    if (name.Contains("minol usa") == true) return ExpenseCategory.Utility;
                    return ExpenseCategory.Special;

                case "Personal Care":
                    return ExpenseCategory.Shopping;

                case "Travel Agents and Tour Operators":
                    return ExpenseCategory.Recreation;

                case "Food and Beverage":
                    return ExpenseCategory.Meal;

                case "Insurance":
                    if (name.Contains("geico") == true) return ExpenseCategory.Vehicle;
                    if (name.Contains("metromile") == true) return ExpenseCategory.Vehicle;
                    return ExpenseCategory.Utility;

                case "Photography":
                    if (name.Contains("the summit at sno") == true) return ExpenseCategory.Recreation;
                    return ExpenseCategory.Utility;

                case "Automotive":
                    return ExpenseCategory.Vehicle;
            }

            return ExpenseCategory.Utility;
        }

        private static ExpenseCategory GetExpenseCategoryShops(string name, string categoryId, string category2, string category3)
        {
            // MSFT payroll
            if (name.Contains("edipayment") == true) return ExpenseCategory.Special;
            if (name.Contains("minol usa") == true) return ExpenseCategory.Utility;
            if (name.Contains("adsense") == true) return ExpenseCategory.Special;

            switch (category2)
            {
                case "Food and Beverage Store":
                    if (name.Contains("chateau ste mich winer") == true) return ExpenseCategory.Grocery;
                    if (name.Contains("ben franklin crafts") == true) return ExpenseCategory.Shopping;
                    return ExpenseCategory.Meal;

                case "Convenience Stores":
                    return ExpenseCategory.Grocery;

                case "Department Stores":
                    if (name == "target") return ExpenseCategory.Grocery;
                    break;

                case "Supermarkets and Groceries":
                    return ExpenseCategory.Grocery;

                case "Warehouses and Wholesale Stores": //costco
                    return ExpenseCategory.Grocery;

                case "Pharmacies":
                    return ExpenseCategory.Others;

                case "Automotive":
                    if (category3 == "Car Dealers and Leasing") return ExpenseCategory.Special; // Auto-Lease
                    return ExpenseCategory.Vehicle;

                case "Musical Instruments":     // Guitar Center - lessons
                    return ExpenseCategory.Recreation;

                case "Music, Video and DVD":    // Play it again sports - snowboard rentals
                    return ExpenseCategory.Recreation;
            }

            return ExpenseCategory.Shopping;
        }

        private static ExpenseCategory GetExpenseCategoryTax(string name, string categoryId, string category2, string category3)
        {
            return ExpenseCategory.Special;
        }

        private static ExpenseCategory GetExpenseCategoryTransfer(string name, string categoryId, string category2, string category3)
        {
            if (name.Contains("ironsteak redmond") == true) return ExpenseCategory.Meal;

            switch (category2)
            {
                // no need to track credit card payments
                case "Credit":
                case "Deposit":
                case "Internal Account Transfer":
                case "Withdrawal":
                case "Wire":
                case "Payroll":
                    return ExpenseCategory.Special;

                case "Debit":
                    if (name.Contains("minol usa") == true) return ExpenseCategory.Utility;
                    return ExpenseCategory.Special;

                case "Third Party":
                    if (category3 == "Venmo") return ExpenseCategory.Others;
                    break;
            }

            return ExpenseCategory.Shopping;
        }

        private static ExpenseCategory GetExpenseCategoryTravel(string name, string categoryId, string category2, string category3)
        {
            if (name.Contains("h-mart") == true) return ExpenseCategory.Grocery;

            switch (category2)
            {
                case "Airlines and Aviation Services": return ExpenseCategory.Recreation;
                case "Lodging": return ExpenseCategory.Recreation;
            }

            return ExpenseCategory.Vehicle;
        }
    }
}
