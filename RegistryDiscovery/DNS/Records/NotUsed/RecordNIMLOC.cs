#region Using Namespaces

using System;

#endregion

public class RecordNIMLOC : Record
{
    #region Public Members

    public byte[] RDATA;

    #endregion

    #region Constructors

    public RecordNIMLOC(RecordReader rr)
	{
		// Re-read length
		ushort RDLENGTH	= rr.Readushort(-2);
		RDATA			= rr.ReadBytes(RDLENGTH);
	}

    #endregion

    #region Public Members

    public override string ToString()
	{
		return "not-used";
	}

    #endregion
}