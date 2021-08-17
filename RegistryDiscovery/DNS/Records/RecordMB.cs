#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
3.3.3. MB RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   MADNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

MADNAME         A <domain-name> which specifies a host which has the
                specified mailbox.

MB records cause additional section processing which looks up an A type
RRs corresponding to MADNAME.
*/

#endregion

public class RecordMB : Record
{
    #region Public Members

    public string MADNAME;

    #endregion

    #region Constructors

    public RecordMB(RecordReader rr)
	{
		MADNAME = rr.ReadDomainName();
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return MADNAME;
	}

    #endregion
}