
namespace Leap.Unity.Query {

  public class LoopOp<SourceType, SourceOp> : IQueryOp<SourceType>
    where SourceOp : IQueryOp<SourceType> {

    private SourceOp _source;

    public LoopOp(SourceOp source) {
      _source = source;
    }

    public bool TryGetNext(out SourceType t) {
      if (_source.TryGetNext(out t)) {
        return true;
      } else {
        _source.Reset();
        if (_source.TryGetNext(out t)) {
          return true;
        } else {
          t = default(SourceType);
          return false;
        }
      }
    }

    public void Reset() {
      _source.Reset();
    }
  }

  public partial struct QueryWrapper<QueryType, QueryOp> where QueryOp : IQueryOp<QueryType> {

    /// <summary>
    /// Returns a new query operation representing the existing elements in the same order,
    /// but repeating forever.  This query operation will never terminate!
    /// 
    /// For example:
    ///   (1, 2, 3).Query().Loop()
    /// Would result in:
    ///   (1, 2, 3, 1, 2, 3, 1, 2, 3... forever)
    /// </summary>
    public QueryWrapper<QueryType, LoopOp<QueryType, QueryOp>> Loop() {
      return new QueryWrapper<QueryType, LoopOp<QueryType, QueryOp>>(new LoopOp<QueryType, QueryOp>(_op));
    }
  }
}
