using UnityEngine;
using UnityEngine.UI;
using eucl3DSDK;
using MaterialUI;
using System.Collections;
using System.Text;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public class Checkout : MonoBehaviour {

    public ScreenManager sm;
    public GameObject creation;
    public Text[] fields;
    public Text[] fieldErrors;
    public Text[] ccFields;
    public Text ccError;
    public decimal modelPrice;
    public decimal total;
    public decimal shipping;
    public decimal taxes;
    public Text subtotalText;
    public Text shippingText;
    public Text taxText;
    public Text totalText;
    public GameObject submitButton;

    public void DrawSizeSelect()
    {
        sm.Set(0);
    }

    public void DrawCustomerInfo()
    {
        sm.Set(1);
    }

    public void DrawPaymentInfo()
    {
        if (ValidateCustomerFields())
        {
            SetOrderPrice();
            sm.Set(2);
        }
    }

    // Performs the three api calls needed to make an order
    // If there is an error with the order, then we show that message to the user
    // If the order is successful then we show a success screen to the user
    public void SubmitOrder()
    {
        ccError.enabled = false;
        submitButton.SetActive(false);

        // While we can't validate a credit card client side, we still should
        // make sure there is no extraneous text in the fields
        SanitizeCreditCard();

        Dictionary<string, string> cc = new Dictionary<string, string>()
        {
            {"number", ccFields[0].text },
            {"cvc", ccFields[1].text },
            {"expMonth", ccFields[2].text },
            {"expYear", ccFields[3].text }
        };

        Dictionary<string, string> customer = new Dictionary<string, string>()
        {
            {"email", fields[2].text },
            {"first", fields[0].text },
            {"last", fields[1].text },
            {"address1", fields[3].text },
            {"address2", fields[8].text },
            {"city", fields[4].text },
            {"state", fields[5].text },
            {"zip", fields[6].text },
            {"country", fields[7].text}
        };

        Dictionary<string, string> price = new Dictionary<string, string>()
        {
            { "totalPrice", total.ToString().Replace(".", "") },
            { "subtotalPrice", modelPrice.ToString().Replace(".", "") },
            { "taxPrice", taxes.ToString().Replace(".", "") },
            { "shippingPrice", shipping.ToString().Replace(".", "") }
        };

        eucl3D.createOrder(cc, customer, price, creation, res =>
        {
            Debug.Log(res.msg);
            Debug.Log(res.text);
            if (res.error)
            {
                ccError.text = res.msg;
                ccError.enabled = true;
                submitButton.SetActive(true);
            }
            else
            {
                Debug.Log(res.msg);
                CompleteOrder();
                submitButton.SetActive(true);

            }
        });

    }

    public void CompleteOrder()
    {
        sm.Set(3);
        for (int i = 0; i < ccFields.Length; i++)
        {
            ccFields[i].text = "";
        }
        for (int i = 0; i < fields.Length; i++)
        {
            fields[i].text = "";
        }
    }


    // We do a basic field validation by making sure no inputs are empty
    // Edge cases: email must contain an @ symbol and Address 2 is optional
    // The Country field has default text so we have to check that as well
    // We make address 2 the last item of the fields array so
    // the fieldErrors array is indexed correctly
    private bool ValidateCustomerFields()
    {
        bool validated = true;

        for (int i = 0; i < fields.Length - 1; i++)
        {

            if (fields[i].text.Length == 0 || fields[i].name == "Email" && !fields[i].text.Contains("@") || fields[i].name == "Country" && fields[i].text == "Country")
            {
                fieldErrors[i].enabled = true;
                validated = false;
            }
            else
            {
                fields[i].text = Regex.Replace(fields[i].text, @"\t|\n|\r", "");
                fieldErrors[i].enabled = false;
            }
        }
        return validated;
    }

    //Performs the same sanization in ValidateCustomerFields, also removes any spaces
    private void SanitizeCreditCard()
    {
        for (int i = 0; i < ccFields.Length; i++)
        {
            ccFields[i].text = Regex.Replace(ccFields[i].text, @"\t|\n|\r", "").Replace(" ", "");
        }

    }

    public void SetSubtotal(string sub)
    {
        modelPrice = Decimal.Parse(sub);
    }

    private void SetOrderPrice()
    {
        shipping = eucl3D.getShippingPrice(modelPrice, fields[7].text);
        taxes = eucl3D.getSalesTax(modelPrice, fields[7].text, fields[5].text);
        total = modelPrice + shipping + taxes;

        subtotalText.text = modelPrice.ToString();
        shippingText.text = shipping.ToString();
        taxText.text = taxes.ToString();
        totalText.text = total.ToString();
    }

}
