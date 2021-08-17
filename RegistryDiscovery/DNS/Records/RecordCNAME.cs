#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
 * 
3.3.1. CNAME RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                     CNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

CNAME           A <domain-name> which specifies the canonical or primary
                name for the owner.  The owner name is an alias.

CNAME RRs cause no additional section processing, but name servers may
choose to restart the query at the canonical name in certain cases.  See
the description of name server logic in [RFC-1034] for details.

 * 
 */

#endregion

public class RecordCNAME : Record
{
    #region Public Members

    public string CNAME;

    #endregion

    #region Constructors

    public RecordCNAME(RecordReader rr)
	{
		CNAME = rr.ReadDomainName();
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return CNAME;
	}

    #endregion
}