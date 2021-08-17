#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
 * http://tools.ietf.org/rfc/rfc2930.txt
 * 
2. The TKEY Resource Record

   The TKEY resource record (RR) has the structure given below.  Its RR
   type code is 249.

      Field       Type         Comment
      -----       ----         -------
       Algorithm:   domain
       Inception:   u_int32_t
       Expiration:  u_int32_t
       Mode:        u_int16_t
       Error:       u_int16_t
       Key Size:    u_int16_t
       Key Data:    octet-stream
       Other Size:  u_int16_t
       Other Data:  octet-stream  undefined by this specification

 */

#endregion

public class RecordTKEY : Record
{
    #region Public Members

    public ushort MODE;
	public ushort ERROR;
	public uint INCEPTION;
	public ushort KEYSIZE;
	public byte[] KEYDATA;
	public uint EXPIRATION;
	public string ALGORITHM;
	public byte[] OTHERDATA;
	public ushort OTHERSIZE;

    #endregion

    #region Constructors

    public RecordTKEY(RecordReader rr)
	{
		ALGORITHM	= rr.ReadDomainName();
		INCEPTION	= rr.Readuint();
		EXPIRATION	= rr.Readuint();
		MODE		= rr.Readushort();
		ERROR		= rr.Readushort();
		KEYSIZE		= rr.Readushort();
		KEYDATA		= rr.ReadBytes(KEYSIZE);
		OTHERSIZE	= rr.Readushort();
		OTHERDATA	= rr.ReadBytes(OTHERSIZE);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return $"{ALGORITHM} {INCEPTION} {EXPIRATION} {MODE} {ERROR}";
	}

    #endregion
}