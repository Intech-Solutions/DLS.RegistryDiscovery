#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
 * http://www.ietf.org/rfc/rfc2845.txt
 * 
 * Field Name       Data Type      Notes
      --------------------------------------------------------------
      Algorithm Name   domain-name    Name of the algorithm
                                      in domain name syntax.
      Time Signed      u_int48_t      seconds since 1-Jan-70 UTC.
      Fudge            u_int16_t      seconds of error permitted
                                      in Time Signed.
      MAC Size         u_int16_t      number of octets in MAC.
      MAC              octet stream   defined by Algorithm Name.
      Original ID      u_int16_t      original message ID
      Error            u_int16_t      expanded RCODE covering
                                      TSIG processing.
      Other Len        u_int16_t      length, in octets, of
                                      Other Data.
      Other Data       octet stream   empty unless Error == BADTIME

 */

#endregion

public class RecordTSIG : Record
{
	#region Public Members

	public byte[] MAC;
	public ushort ERROR;
	public ushort FUDGE;
	public ushort MACSIZE;
	public long TIMESIGNED;
	public ushort OTHERLEN;
	public byte[] OTHERDATA;
	public ushort ORIGINALID;
	public string ALGORITHMNAME;

	#endregion

    #region Constructors

    public RecordTSIG(RecordReader rr)
	{
		ALGORITHMNAME	= rr.ReadDomainName();
		TIMESIGNED		= rr.Readuint() << 32 | rr.Readuint();
		FUDGE			= rr.Readushort();
		MACSIZE			= rr.Readushort();
		MAC				= rr.ReadBytes(MACSIZE);
		ORIGINALID		= rr.Readushort();
		ERROR			= rr.Readushort();
		OTHERLEN		= rr.Readushort();
		OTHERDATA		= rr.ReadBytes(OTHERLEN);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		DateTime dateTime	= new DateTime(1970, 1, 1, 0, 0, 0, 0);
		dateTime			= dateTime.AddSeconds(TIMESIGNED);
		string printDate	= $"{dateTime.ToShortDateString()} {dateTime.ToShortTimeString()}";

		return $"{ALGORITHMNAME} {printDate} {FUDGE} {ORIGINALID} {ERROR}";
	}
	
	#endregion
}