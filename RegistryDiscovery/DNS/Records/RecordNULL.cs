#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
3.3.10. NULL RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                  <anything>                   /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

Anything at all may be in the RDATA field so long as it is 65535 octets
or less.

NULL records cause no additional section processing.  NULL RRs are not
allowed in master files.  NULLs are used as placeholders in some
experimental extensions of the DNS.
*/

#endregion

public class RecordNULL : Record
{
    #region Public Members

    public byte[] ANYTHING;

    #endregion

    #region Constructors

    public RecordNULL(RecordReader rr)
	{
		rr.Position -= 2;
		// Re-read length
		ushort RDLENGTH	= rr.Readushort();
		ANYTHING		= new byte[RDLENGTH];
		ANYTHING		= rr.ReadBytes(RDLENGTH);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return $"...binary data... ({ANYTHING.Length}) bytes";
	}

    #endregion
}