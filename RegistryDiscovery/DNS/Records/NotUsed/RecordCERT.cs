#region Using Namespaces

using System;

#endregion

public class RecordCERT : Record
{
    #region Public Members

    public byte[] RDATA;

    #endregion

    #region Construcors

    public RecordCERT(RecordReader rr)
	{
		// Re-read length
		ushort RDLENGTH = rr.Readushort(-2);
		RDATA           = rr.ReadBytes(RDLENGTH);
	}

    #endregion

    #region

    public override string ToString()
	{
		return "not-used";
	}

    #endregion
}