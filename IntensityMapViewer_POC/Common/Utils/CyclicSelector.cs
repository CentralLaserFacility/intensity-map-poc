//
// CyclicSelector.cs
//

using System.Collections.Generic ;
using System.Linq ;

namespace Common
{

  //
  // This lets you cycle through a sequence of instances
  // that are specified in the constructor.
  //

  public class CyclicSelector<T>
  {

    private IReadOnlyList<T> m_items ;

    private int m_currentItemIndex = 0 ;

    public CyclicSelector ( IEnumerable<T> items )
    {
      m_items = items.ToList() ;
    }

    public CyclicSelector ( params T[] items )
    {
      m_items = items.ToList() ;
    }

    public void Reset ( )
    {
      m_currentItemIndex = 0 ;
    }

    public T GetCurrent_MoveNext ( )
    {
      T currentItem = m_items[m_currentItemIndex] ;
      m_currentItemIndex++ ;
      if ( m_currentItemIndex == m_items.Count )
      {
        m_currentItemIndex = 0 ;
      }
      return currentItem ;
    }

    public T Current => m_items[m_currentItemIndex] ;

    public void MoveNext ( )
    {
      m_currentItemIndex++ ;
      if ( m_currentItemIndex == m_items.Count )
      {
        m_currentItemIndex = 0 ;
      }
    }

  }

}
