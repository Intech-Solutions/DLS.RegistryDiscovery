#region Using Namespaces

using System.Net;

#endregion

#region RFC Info

/*
2.2 AAAA data format

   A 128 bit IPv6 address is encoded in the data portion of an AAAA
   resource record in network byte order (high-order byte first).
 */

#endregion


public class RecordAAAA : Record
{
    #region Public Members

    public IPAddress Address;

    #endregion

    #region Constructors

    public RecordAAAA(RecordReader rr)
	{
		IPAddress.TryParse(
			string.Format("{0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}:{6:x}:{7:x}",
			rr.Readushort(),
			rr.Readushort(),
			rr.Readushort(),
			rr.Readushort(),
			rr.Readushort(),
			rr.Readushort(),
			rr.Readushort(),
			rr.Readushort()), out Address);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return Address.ToString();
	}

    #endregion
}