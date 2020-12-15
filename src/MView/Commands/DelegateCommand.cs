using System;
using System.Windows.Input;

namespace MView.Commands
{
    /// <summary>
    /// 대리자를 이용해 다른 메소드를 실행해주는 커맨드 클래스입니다.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private readonly Func<bool> _canExecute;
        private readonly Action _execute;

        /// <summary>
        /// DelegateCommand 클래스의 새 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="execute">실행 할 메소드입니다.</param>
        public DelegateCommand(Action execute) : this(execute, null)
        {

        }

        /// <summary>
        /// DelegateCommand 클래스의 새 인스턴스를 생성하고 초기화합니다.
        /// </summary>
        /// <param name="execute">실행 할 메소드입니다.</param>
        /// <param name="canExecute">커맨드가 실행 가능한지에 대한 여부를 반환하는 메소드입니다.</param>
        public DelegateCommand(Action execute, Func<bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        /// <summary>
        /// 커맨드의 실행 가능 여부가 변경되었을 때 호출되는 이벤트입니다.
        /// </summary>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// 커맨드가 실행 가능한지에 대한 여부를 반환합니다.
        /// </summary>
        /// <param name="obj">메소드를 실행하기에 앞서 넘겨줄 인수입니다.</param>
        /// <returns>커맨드 실행 가능 여부.</returns>
        public bool CanExecute(object obj)
        {
            if (_canExecute == null)
            {
                return true;
            }
            return _canExecute();
        }

        /// <summary>
        /// 지정된 메소드를 실행합니다.
        /// </summary>
        /// <param name="obj">메소드를 실행하기에 앞서 넘겨줄 인수입니다.</param>
        public void Execute(object obj)
        {
            _execute();
        }

        /// <summary>
        /// 커맨드 실행 가능 여부가 변경되었음을 알립니다.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
