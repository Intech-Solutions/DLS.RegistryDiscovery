#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
 * http://tools.ietf.org/rfc/rfc2672.txt
 * 
3. The DNAME Resource Record

   The DNAME RR has mnemonic DNAME and type code 39 (decimal).
   DNAME has the following format:

      <owner> <ttl> <class> DNAME <target>

   The format is not class-sensitive.  All fields are required.  The
   RDATA field <target> is a <domain-name> [DNSIS].

 * 
 */

#endregion

public class RecordDNAME : Record
{
    #region Public Members

    public string TARGET;

    #endregion

    #region Constructors

    public RecordDNAME(RecordReader rr)
	{
		TARGET = rr.ReadDomainName();
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return TARGET;
	}

    #endregion
}