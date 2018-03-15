using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceBorderless.Classes
{
    /// <summary>
    /// Class used to stocked default values of modified hWnd
    /// </summary>
    public class hWndInfos
    {
        #region Properties

        /// <summary>
        /// hWnd ID
        /// </summary>
        private IntPtr _id;

        /// <summary>
        /// Window Style
        /// </summary>
        private int _Style;

        /// <summary>
        /// Window X screen location
        /// </summary>
        private int _LocationX;

        /// <summary>
        /// Window Y screen location
        /// </summary>
        private int _LocationY;

        /// <summary>
        /// Window width
        /// </summary>
        private int _Width;

        /// <summary>
        /// Window height
        /// </summary>
        private int _Height;

        #endregion Properties

        #region Accessors

        /// <summary>
        /// hWnd ID
        /// </summary>
        public IntPtr id
        {
            get { return _id; }
            set { _id = value; }
        }

        /// <summary>
        /// Window Style
        /// </summary>
        public int Style
        {
            get { return _Style; }
            set { _Style = value; }
        }

        /// <summary>
        /// Window X screen location
        /// </summary>
        public int LocationX
        {
            get { return _LocationX; }
            set { _LocationX = value; }
        }

        /// <summary>
        /// Window Y screen location
        /// </summary>
        public int LocationY
        {
            get { return _LocationY; }
            set { _LocationY = value; }
        }

        /// <summary>
        /// Window width
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }

        /// <summary>
        /// Window height
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }

        #endregion Accessors

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="id">hWnd id</param>
        /// <param name="Style">Window style</param>
        /// <param name="LocationX">Window X screen location</param>
        /// <param name="LocationY">Window Y screen location</param>
        /// <param name="Width">Window width</param>
        /// <param name="Height">Window height</param>
        public hWndInfos(IntPtr id, int Style, int LocationX, int LocationY, int Width, int Height)
        {
            this._id = id;
            this._Style = Style;
            this._LocationX = LocationX;
            this._LocationY = LocationY;
            this._Width = Width;
            this._Height = Height;
        }

        #endregion Constructor
    }
}
