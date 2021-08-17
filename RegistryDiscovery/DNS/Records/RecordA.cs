#region Using Namespaces

using System.Net;

#endregion

#region RFC Info

/*
 3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
 * 
 */

#endregion

public class RecordA : Record
{
    #region Public Members

    public IPAddress Address;

    #endregion

    #region Constructors

    public RecordA(RecordReader rr)
	{
		Address = new IPAddress(rr.ReadBytes(4));
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return Address.ToString();
	}

    #endregion
}