#region Using Namespaces

using System;

#endregion

public class RecordUINFO : Record
{
    #region Public Members

    public byte[] RDATA;

    #endregion

    #region Constructors

    public RecordUINFO(RecordReader rr)
	{
		// Re-read length
		ushort RDLENGTH = rr.Readushort(-2);
		RDATA           = rr.ReadBytes(RDLENGTH);
	}

    #endregion

    #region Public Methods

    public override string ToString()
	{
		return string.Format("not-used");
	}

    #endregion
}