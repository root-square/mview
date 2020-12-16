using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MView.Bases
{
    /// <summary>
    /// 모든 ViewModel의 기본이 되는 INotifyPropertyChanged 구현체입니다.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// 속성 값이 변경되었을 때 호출되는 이벤트입니다.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 속성 값이 변경되었음을 알립니다.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            // take a copy to prevent thread issues.
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
