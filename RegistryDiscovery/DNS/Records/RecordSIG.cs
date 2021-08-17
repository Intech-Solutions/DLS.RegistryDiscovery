#region Using Namespaces

using System;

#endregion

#region RFC Info

/*
 * http://www.ietf.org/rfc/rfc2535.txt
 * 4.1 SIG RDATA Format

   The RDATA portion of a SIG RR is as shown below.  The integrity of
   the RDATA information is protected by the signature field.

                           1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |        type covered           |  algorithm    |     labels    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                         original TTL                          |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      signature expiration                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      signature inception                      |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |            key  tag           |                               |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+         signer's name         +
      |                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-/
      /                                                               /
      /                            signature                          /
      /                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+


*/

#endregion

public class RecordSIG : Record
{
    #region Public Members

	public byte LABELS;
	public ushort KEYTAG;
	public byte ALGORITHM;
	public uint ORIGINALTTL;
	public string SIGNATURE;
	public string SIGNERSNAME;
	public ushort TYPECOVERED;
	public uint SIGNATUREINCEPTION;
	public uint SIGNATUREEXPIRATION;

    #endregion

    #region Constructors

    public RecordSIG(RecordReader rr)
	{
		TYPECOVERED			= rr.Readushort();
		ALGORITHM			= rr.ReadByte();
		LABELS				= rr.ReadByte();
		ORIGINALTTL			= rr.Readuint();
		SIGNATUREEXPIRATION	= rr.Readuint();
		SIGNATUREINCEPTION	= rr.Readuint();
		KEYTAG				= rr.Readushort();
		SIGNERSNAME			= rr.ReadDomainName();
		SIGNATURE			= rr.ReadString();
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return $"{TYPECOVERED} {ALGORITHM} {LABELS} {ORIGINALTTL} {SIGNATUREEXPIRATION} {SIGNATUREINCEPTION} {KEYTAG} {SIGNERSNAME} \"{SIGNATURE}\"";
	}

    #endregion
}