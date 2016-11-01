
public class ProduceConsumeBuffer<T> {
  private T[] _buffer;
  private uint _bufferMask;
  private uint _head, _tail;

  public ProduceConsumeBuffer(int size) {
    _buffer = new T[size];
    _bufferMask = (uint)(size - 1);
    _head = 0;
    _tail = 0;
  }

  public int Capacity {
    get {
      return _buffer.Length;
    }
  }

  public bool TryPush(ref T t) {
    uint nextTail = (_tail + 1) & _bufferMask;
    if (nextTail == _head) return false;

    _buffer[_tail] = t;
    _tail = nextTail;
    return true;
  }

  public bool TryPop(out T t) {
    if (_tail == _head) {
      t = default(T);
      return false;
    }

    t = _buffer[_head];
    _head = (_head + 1) & _bufferMask;
    return true;
  }
}
