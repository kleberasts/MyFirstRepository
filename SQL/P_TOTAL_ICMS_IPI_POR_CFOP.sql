USE [Teste]
GO
IF OBJECT_ID('dbo.P_TOTAL_ICMS_IPI_POR_CFOP') IS NOT NULL
BEGIN
    DROP PROCEDURE dbo.P_TOTAL_ICMS_IPI_POR_CFOP
    IF OBJECT_ID('dbo.P_TOTAL_ICMS_IPI_POR_CFOP') IS NOT NULL
        PRINT '<<< FALHA APAGANDO A PROCEDURE dbo.P_TOTAL_ICMS_IPI_POR_CFOP >>>'
    ELSE
        PRINT '<<< PROCEDURE dbo.P_TOTAL_ICMS_IPI_POR_CFOP APAGADA >>>'
END
go
SET QUOTED_IDENTIFIER ON
GO
SET NOCOUNT ON 
GO 
CREATE PROCEDURE P_TOTAL_ICMS_IPI_POR_CFOP
AS
BEGIN
	SELECT CFOP,
		   SUM(ISNULL(BaseIcms, 0)) ValTotBaseIcms,
		   SUM(ISNULL(ValorIcms, 0)) ValTotIcms,
		   SUM(ISNULL(BaseIpi, 0)) ValTotBaseIpi,
		   SUM(ISNULL(ValorIpi, 0)) ValTotIpi
	FROM NotaFiscalItem
	GROUP BY CFOP
END
GO

GRANT EXECUTE ON dbo.P_TOTAL_ICMS_IPI_POR_CFOP TO [public]
go
IF OBJECT_ID('dbo.P_TOTAL_ICMS_IPI_POR_CFOP') IS NOT NULL
    PRINT '<<< PROCEDURE dbo.P_TOTAL_ICMS_IPI_POR_CFOP CRIADA >>>'
ELSE
    PRINT '<<< FALHA NA CRIACAO DA PROCEDURE dbo.P_TOTAL_ICMS_IPI_POR_CFOP >>>'
go