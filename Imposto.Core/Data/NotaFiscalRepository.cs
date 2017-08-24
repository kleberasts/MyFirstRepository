using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;

namespace Imposto.Core.Data
{
    public class NotaFiscalRepository : IRepository<NotaFiscal>, IDisposable
    {
        private EntityModelContext _context;

        public NotaFiscalRepository()
        {
            System.Data.Entity.Database.SetInitializer<EntityModelContext>(null);
            _context = new EntityModelContext();
        }

        public NotaFiscal[] GetAll()
        {
            return _context.eNotaFiscal.ToArray();
        }

        public NotaFiscal GetById(int id)
        {
            return _context.eNotaFiscal.Where(s => s.Id == id).FirstOrDefault();
        }

        public List<NotaFiscal> GetByCostumerName(string CostumerName)
        {
            List<NotaFiscal> nfsCliente = _context.eNotaFiscal.Where(s => s.NomeCliente.Contains(CostumerName)).ToList();

            return nfsCliente;
        }

        public IQueryable<NotaFiscal> Query(Expression<Func<NotaFiscal, bool>> filter)
        {
            return _context.eNotaFiscal.Where(filter);
        }

        public void Save(NotaFiscal entity)
        {
            var IdRetorno = new SqlParameter("@pId", System.Data.SqlDbType.Int) { Direction = System.Data.ParameterDirection.InputOutput, SqlValue = entity.Id };
            
            _context.Database.ExecuteSqlCommand("EXEC P_NOTA_FISCAL @pId OUTPUT, @pNumeroNotaFiscal, @pSerie, @pNomeCliente, @pEstadoDestino, @pEstadoOrigem",
                                                IdRetorno,
                                                new SqlParameter("@pNumeroNotaFiscal", entity.NumeroNotaFiscal),
                                                new SqlParameter("@pSerie", entity.Serie),
                                                new SqlParameter("@pNomeCliente", entity.NomeCliente),
                                                new SqlParameter("@pEstadoDestino", entity.EstadoDestino),
                                                new SqlParameter("@pEstadoOrigem", entity.EstadoOrigem)
                                               );
            entity.Id = Convert.ToInt32(IdRetorno.Value);

            foreach (NotaFiscalItem itemNF in entity.ItensDaNotaFiscal)
            {
                itemNF.IdNotaFiscal = entity.Id;
                _context.Database.ExecuteSqlCommand("EXEC P_NOTA_FISCAL_Item @pId, @pIdNotaFiscal, @pCfop, @pTipoIcms, @pBaseIcms, @pAliquotaIcms, @pValorIcms, @pBaseIpi, @pAliquotaIpi, @pValorIpi, @pDesconto, @pNomeProduto, @pCodigoProduto",
                                                    new SqlParameter("@pId", itemNF.Id),
                                                    new SqlParameter("@pIdNotaFiscal", itemNF.IdNotaFiscal),
                                                    new SqlParameter("@pCfop", !string.IsNullOrEmpty(itemNF.Cfop)? itemNF.Cfop : string.Empty),
                                                    new SqlParameter("@pTipoIcms", itemNF.TipoIcms),
                                                    new SqlParameter("@pBaseIcms", itemNF.BaseIcms),
                                                    new SqlParameter("@pAliquotaIcms", itemNF.AliquotaIcms),
                                                    new SqlParameter("@pValorIcms", itemNF.ValorIcms),
                                                    new SqlParameter("@pBaseIpi", itemNF.BaseIpi),
                                                    new SqlParameter("@pAliquotaIpi", itemNF.AliquotaIpi),
                                                    new SqlParameter("@pValorIpi", itemNF.ValorIpi),
                                                    new SqlParameter("@pDesconto", itemNF.Desconto),
                                                    new SqlParameter("@pNomeProduto", itemNF.NomeProduto),
                                                    new SqlParameter("@pCodigoProduto", itemNF.CodigoProduto)
                                                   );
                _context.SaveChanges();
            }
        }

        public void Delete(NotaFiscal entity)
        {
            _context.eNotaFiscal.Remove(entity);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}
