using System.Xml.Serialization;

[XmlRoot("FROMPOS")]
public class PosReceipt
{
    [XmlElement("AuthNum")]
    public string? AuthorizationNumber { get; set; }

    [XmlElement("RefNum")]
    public string? ReferenceNumber { get; set; }

    [XmlElement("ReceiptNum")]
    public string? ReceiptNumber { get; set; }

    [XmlElement("MerchID")]
    public string? MerchantId { get; set; }

    [XmlElement("TermID")]
    public string? TerminalId { get; set; }

    [XmlElement("CardMask")]
    public string? CardMask { get; set; }

    [XmlElement("CardVerified")]
    public string? CardVerified { get; set; }

    [XmlElement("TrxStatusMsg")]
    public string? TransactionStatus { get; set; }

    [XmlElement("ResponseCode")]
    public string? ResponseCode { get; set; }

    [XmlElement("TrxDate")]
    public string? TransactionDate { get; set; }

    [XmlElement("TrxTime")]
    public string? TransactionTime { get; set; }

    public bool IsApproved => ResponseCode == "00";
}