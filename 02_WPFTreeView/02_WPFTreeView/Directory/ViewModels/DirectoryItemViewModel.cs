﻿using _02_WPFTreeView.Directory.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace _02_WPFTreeView
{
    /// <summary>
    /// A view model for each directory item.
    /// </summary>
    public class DirectoryItemViewModel:BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// The type of this item
        /// </summary>
        public DirectoryItemType Type { get; set; }
        /// <summary>
        /// The absolute path of this item
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// The name of this directory item
        /// </summary>
        public string Name => this.Type == DirectoryItemType.Drive ? 
            this.FullPath : DirectoryStructure.GetFileFolderName(this.FullPath);
        
        /// <summary>
        /// A list of all children contained inside this item 
        /// </summary>
        public ObservableCollection<DirectoryItemViewModel> Children { get; set; }

        /// <summary>
        /// Indicates if this item can be expanded
        /// </summary>
        public bool CanExpand { get { return this.Type != DirectoryItemType.File; } }

        /// <summary>
        /// Indicates if the current item is expand or not
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.Children?.Count(f => f != null) > 0;
            }
            set
            {
                // If the UI tells us to expand
                if (value == true)
                    // Find all children
                    Expand();
                else
                    this.ClearChildren();
            }
        }

        #endregion

        #region Public commands
        /// <summary>
        /// The Command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fullPath">full path of this item</param>
        /// <param name="type">type of this item</param>
        public DirectoryItemViewModel(string fullPath, DirectoryItemType type)
        {
            // create command
            this.ExpandCommand = new RelayCommand(Expand);

            // Set full path and type
            this.FullPath = fullPath;
            this.Type = type;

            // Set children as needed
            this.ClearChildren();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Removes all children from the list, adding a 
        /// dummy item to show the expand icon if required
        /// </summary>
        private void ClearChildren()
        {
            // Clear items
            this.Children = new ObservableCollection<DirectoryItemViewModel>();

            // Show the expand arrow if we are not a file
            if (this.Type != DirectoryItemType.File)
            {
                this.Children.Add(null);
            }
        }

        #endregion
        /// <summary>
        /// Expands this directory and find all children
        /// </summary>
        private void Expand()
        {
            // We can't expand a file
            if (this.Type == DirectoryItemType.File)
                return;

            // Find all children
            this.Children = new ObservableCollection<DirectoryItemViewModel>
                (DirectoryStructure.GetDirectoryContents(this.FullPath).Select(content =>
                new DirectoryItemViewModel(content.FullPath, content.Type)));
        }
    }
}
