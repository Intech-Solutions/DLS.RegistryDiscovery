#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
3.3.6. MG RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   MGMNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

MGMNAME         A <domain-name> which specifies a mailbox which is a
                member of the mail group specified by the domain name.

MG records cause no additional section processing.
*/

#endregion

public class RecordMG : Record
{
    #region Public Members

    public string MGMNAME;

    #endregion

    #region Constructors

    public RecordMG(RecordReader rr)
	{
		MGMNAME = rr.ReadDomainName();
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return MGMNAME;
	}

    #endregion
}