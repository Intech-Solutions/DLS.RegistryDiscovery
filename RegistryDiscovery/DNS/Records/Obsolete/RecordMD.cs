#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
3.3.4. MD RDATA format (Obsolete)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   MADNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

MADNAME         A <domain-name> which specifies a host which has a mail
                agent for the domain which should be able to deliver
                mail for the domain.

MD records cause additional section processing which looks up an A type
record corresponding to MADNAME.

MD is obsolete.  See the definition of MX and [RFC-974] for details of
the new scheme.  The recommended policy for dealing with MD RRs found in
a master file is to reject them, or to convert them to MX RRs with a
preference of 0.
 * */

#endregion

public class RecordMD : Record
{
    #region Public Members

    public string MADNAME;

    #endregion

    #region Constructors

    public RecordMD(RecordReader rr)
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