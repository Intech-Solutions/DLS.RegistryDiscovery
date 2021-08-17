#region Using Namespaces

using System.Text;

#endregion

#region RFC Info

/*
 * http://tools.ietf.org/rfc/rfc3658.txt
 * 
2.4.  Wire Format of the DS record

   The DS (type=43) record contains these fields: key tag, algorithm,
   digest type, and the digest of a public key KEY record that is
   allowed and/or used to sign the child's apex KEY RRset.  Other keys
   MAY sign the child's apex KEY RRset.

                        1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |           key tag             |  algorithm    |  Digest type  |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                digest  (length depends on type)               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                (SHA-1 digest is 20 bytes)                     |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

 */

#endregion

public class RecordDS : Record
{
    #region Public Members

    public byte[] DIGEST;
	public ushort KEYTAG;
	public byte ALGORITHM;
	public byte DIGESTTYPE;

    #endregion

    #region Constructor

    public RecordDS(RecordReader rr)
	{
		ushort length	= rr.Readushort(-2);
		KEYTAG			= rr.Readushort();
		ALGORITHM		= rr.ReadByte();
		DIGESTTYPE		= rr.ReadByte();
		length			-= 4;
		DIGEST			= new byte[length];
		DIGEST			= rr.ReadBytes(length);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		StringBuilder sb = new StringBuilder();
		
		for (int intI = 0; intI < DIGEST.Length; intI++)
			sb.Append($"{DIGEST[intI]:x2}");

		return $"{KEYTAG} {ALGORITHM} {DIGESTTYPE} {sb}";
	}

    #endregion
}