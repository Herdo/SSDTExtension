﻿namespace SSDTLifecycleExtension.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Windows.Input;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.PlatformUI;
    using MVVM;
    using Shared.Contracts;
    using Shared.Contracts.DataAccess;
    using Shared.Contracts.Services;
    using Shared.Models;
    using Task = System.Threading.Tasks.Task;

    [UsedImplicitly]
    public class ConfigurationViewModel : ViewModelBase,
                                          IErrorHandler
    {
        private readonly SqlProject _project;
        private readonly IConfigurationService _configurationService;
        private readonly IFileSystemAccess _fileSystemAccess;
        private readonly IScaffoldingService _scaffoldingService;
        private readonly IScriptCreationService _scriptCreationService;
        private readonly ILogger _logger;

        private ConfigurationModel _lastSavedModel;
        private ConfigurationModel _model;
        private bool _isModelDirty;

        public ConfigurationModel Model
        {
            get => _model;
            set
            {
                if (Equals(value, _model)) return;
                if (_model != null)
                    _model.PropertyChanged -= Model_PropertyChanged;
                _model = value;
                if (_model != null)
                    _model.PropertyChanged += Model_PropertyChanged;
                OnPropertyChanged();
                CheckIfModelIsDirty();
            }
        }

        private bool IsModelDirty
        {
            get => _isModelDirty;
            set
            {
                if (value == _isModelDirty) return;
                _isModelDirty = value;
                SaveConfigurationCommand.RaiseCanExecuteChanged();
            }
        }
        public ICommand BrowsePublishProfileCommand { get; }
        public ICommand ResetConfigurationToDefaultCommand { get; }
        public IAsyncCommand SaveConfigurationCommand { get; }

        public ConfigurationViewModel(SqlProject project,
                                      IConfigurationService configurationService,
                                      IFileSystemAccess fileSystemAccess,
                                      IScaffoldingService scaffoldingService,
                                      IScriptCreationService scriptCreationService,
                                      ILogger logger)
        {
            _project = project;
            _configurationService = configurationService;
            _fileSystemAccess = fileSystemAccess;
            _scaffoldingService = scaffoldingService;
            _scriptCreationService = scriptCreationService;
            _logger = logger;
            _scaffoldingService.IsScaffoldingChanged += ScaffoldingService_IsScaffoldingChanged;
            _scriptCreationService.IsCreatingChanged += ScriptCreationService_IsCreatingChanged;

            BrowsePublishProfileCommand = new DelegateCommand(BrowsePublishProfile_Executed);
            ResetConfigurationToDefaultCommand = new DelegateCommand(ResetConfigurationToDefault_Executed);
            SaveConfigurationCommand = new AsyncCommand(SaveConfiguration_ExecutedAsync, SaveConfiguration_CanExecute, this);
        }

        private void BrowsePublishProfile_Executed()
        {
            var browsedPath = _fileSystemAccess.BrowseForFile(".publish.xml", "Publish profile (*.publish.xml)|*.publish.xml");
            if (browsedPath != null)
            {
                var projectPath = new Uri(_project.FullName, UriKind.Absolute);
                var profilePath = new Uri(browsedPath, UriKind.Absolute);
                var relativePath = projectPath.MakeRelativeUri(profilePath).ToString();
                Model.PublishProfilePath = relativePath;
            }
        }

        private void ResetConfigurationToDefault_Executed()
        {
            Model = ConfigurationModel.GetDefault();
            Model.ValidateAll();
        }

        private bool SaveConfiguration_CanExecute()
        {
            return Model != null
                && !Model.HasErrors
                && IsModelDirty
                && !_scaffoldingService.IsScaffolding
                && !_scriptCreationService.IsCreating;
        }

        private async Task SaveConfiguration_ExecutedAsync()
        {
            var copy = Model.Copy();
            await _configurationService.SaveConfigurationAsync(_project, copy);
            _lastSavedModel = copy;
            CheckIfModelIsDirty();
        }

        private void CheckIfModelIsDirty()
        {
            if (Model == null)
            {
                IsModelDirty = false;
                return;
            }

            if (Model != null && _lastSavedModel == null)
            {
                IsModelDirty = true;
                return;
            }

            // Check by properties
            IsModelDirty = Model.ArtifactsPath != _lastSavedModel.ArtifactsPath
                           || Model.PublishProfilePath != _lastSavedModel.PublishProfilePath
                           || Model.BuildBeforeScriptCreation != _lastSavedModel.BuildBeforeScriptCreation
                           || Model.CreateDocumentationWithScriptCreation != _lastSavedModel.CreateDocumentationWithScriptCreation
                           || Model.CommentOutReferencedProjectRefactorings != _lastSavedModel.CommentOutReferencedProjectRefactorings
                           || Model.CommentOutUnnamedDefaultConstraintDrops != _lastSavedModel.CommentOutUnnamedDefaultConstraintDrops
                           || Model.ReplaceUnnamedDefaultConstraintDrops != _lastSavedModel.ReplaceUnnamedDefaultConstraintDrops
                           || Model.VersionPattern != _lastSavedModel.VersionPattern
                           || Model.TrackDacpacVersion != _lastSavedModel.TrackDacpacVersion
                           || Model.CustomHeader != _lastSavedModel.CustomHeader
                           || Model.CustomFooter != _lastSavedModel.CustomFooter;
        }

        public async Task InitializeAsync()
        {
            _lastSavedModel = await _configurationService.GetConfigurationOrDefaultAsync(_project);
            Model = _lastSavedModel.Copy();
        }

        private void ScaffoldingService_IsScaffoldingChanged(object sender, EventArgs e)
        {
            SaveConfigurationCommand.RaiseCanExecuteChanged();
        }

        private void ScriptCreationService_IsCreatingChanged(object sender, EventArgs e)
        {
            SaveConfigurationCommand.RaiseCanExecuteChanged();
        }

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckIfModelIsDirty();
        }

        void IErrorHandler.HandleError(IAsyncCommand command, Exception exception)
        {
            if (!ReferenceEquals(command, SaveConfigurationCommand))
                throw new NotSupportedException();

            try
            {
                _logger.LogAsync($"Error during execution of {nameof(SaveConfigurationCommand)}: {exception}").RunSynchronously();
            }
            catch
            {
                // ignored - when logging the exception fails, we don't want to end up in a stack overflow.
            }
        }
    }
}