﻿// <copyright file="SelectionSteps.cs">
//    Copyright © 2013 Dan Piessens.  All rights reserved.
// </copyright>

namespace SpecBind
{
    using SpecBind.ActionPipeline;
    using SpecBind.Actions;
    using SpecBind.Helpers;
    using SpecBind.Pages;
    using TechTalk.SpecFlow;

    /// <summary>
    /// A set of step bindings for selecting an item.
    /// </summary>
    [Binding]
    public class SelectionSteps : PageStepBase
    {
        // Step regex values - in constants because they are shared.
        private const string ChooseALinkStepRegex = @"I choose (.+)";
        private const string ChooseAnExplicitLinkStepRegex = @"I click (?:the )?(?:"")?([^""]*)(?:"")?";
        private const string HoverOverAnElementStepRegex = @"I hover over (.+)";
        private const string EnsureOnListItemRegex = @"I am on list (.*) item ([0-9]+)";
        private const string GoToListItemWithCriteriaRegex = @"I am on (.*) list item matching criteria";

        // The following Regex items are for the given "past tense" form
        private const string GivenChooseALinkStepRegex = @"I chose (.+)";
        private const string GivenChooseAnExplicitLinkStepRegex = @"I clicked (?:the )?(?:"")?([^""]*)(?:"")?";
        private const string GivenDoubleClickedStepRegex = @"I double-clicked (?:the )?(?:"")?([^""]*)(?:"")?";
        private const string WhenDoubleClickStepRegex = @"I double-click (?:the )?(?:"")?([^""]*)(?:"")?";
        private const string RightClickedStepRegex = @"I right-click(?:ed)? (?:the )?(?:"")?([^""]*)(?:"")?";
        private const string GivenHoverOverAnElementStepRegex = @"I hovered over (.+)";
        private const string GivenEnsureOnListItemRegex = @"I was on list (.*) item ([0-9]+)";
        private const string GivenGoToListItemWithCriteriaRegex = @"I was on (.*) list item matching criteria";

        private readonly IActionPipelineService actionPipelineService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionSteps" /> class.
        /// </summary>
        /// <param name="actionPipelineService">The action pipeline service.</param>
        /// <param name="scenarioContext">The scenario context.</param>
        /// <param name="logger">The logger.</param>
        public SelectionSteps(
            IActionPipelineService actionPipelineService,
            IScenarioContextHelper scenarioContext,
            ILogger logger)
            : base(scenarioContext, logger)
        {
            this.actionPipelineService = actionPipelineService;
        }

        /// <summary>
        /// A step to click a link with an explicit name.
        /// </summary>
        /// <param name="linkName">Name of the link.</param>
        [Given(GivenChooseAnExplicitLinkStepRegex)]
        [When(ChooseAnExplicitLinkStepRegex)]
        public void WhenIChooseAnExplicitLinkStep(string linkName)
        {
            this.ClickAction<ButtonClickAction>(linkName.ToIdentifier());
        }

        /// <summary>
        /// A step to double-click an element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        [Given(GivenDoubleClickedStepRegex)]
        [When(WhenDoubleClickStepRegex)]
        public void DoubleClickStep(string elementName)
        {
            this.ClickAction<ButtonDoubleClickAction>(elementName.ToIdentifier());
        }

        /// <summary>
        /// A step to right-click an element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        [Given(RightClickedStepRegex)]
        [When(RightClickedStepRegex)]
        public void RightClickStep(string elementName)
        {
            this.ClickAction<ButtonRightClickAction>(elementName.ToIdentifier());
        }

        /// <summary>
        /// A step to click a link.
        /// </summary>
        /// <param name="linkName">Name of the link.</param>
        [Given(GivenChooseALinkStepRegex)]
        [When(ChooseALinkStepRegex)]
        public void WhenIChooseALinkStep(string linkName)
        {
            this.ClickAction<ButtonClickAction>(linkName.ToLookupKey());
        }

        /// <summary>
        /// A When step indicating a link click should occur.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        [Given(GivenHoverOverAnElementStepRegex)]
        [When(HoverOverAnElementStepRegex)]
        public void WhenIHoverOverAnElementStep(string elementName)
        {
            var page = this.GetPageFromContext();

            var context = new ActionContext(elementName.ToLookupKey());

            this.actionPipelineService
                    .PerformAction<HoverOverElementAction>(page, context)
                    .CheckResult();
        }

        /// <summary>
        /// A Given step for ensuring the browser is on the list item with the specified name and index.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="itemNumber">The item number.</param>
        [Given(GivenEnsureOnListItemRegex)]
        [When(EnsureOnListItemRegex)]
        [Then(EnsureOnListItemRegex)]
        public void GivenEnsureOnListItemStep(string listName, int itemNumber)
        {
            var page = this.GetPageFromContext();

            var context = new GetListItemByIndexAction.ListItemByIndexContext(listName.ToLookupKey(), itemNumber);

            var item = this.actionPipelineService.PerformAction<GetListItemByIndexAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
        }

        /// <summary>
        /// A step for ensuring the browser is on the list item with the specified name and criteria.
        /// </summary>
        /// <param name="listName">Name of the list.</param>
        /// <param name="criteriaTable">The criteria table.</param>
        [Given(GivenGoToListItemWithCriteriaRegex)]
        [When(GoToListItemWithCriteriaRegex)]
        [Then(GoToListItemWithCriteriaRegex)]
        public void GoToListItemWithCriteriaStep(string listName, Table criteriaTable)
        {
            var page = this.GetPageFromContext();
            var validationTable = criteriaTable.ToValidationTable();

            var context = new GetListItemByCriteriaAction.ListItemByCriteriaContext(listName.ToLookupKey(), validationTable);

            var item = this.actionPipelineService.PerformAction<GetListItemByCriteriaAction>(page, context)
                                                 .CheckResult<IPage>();

            this.UpdatePageContext(item);
        }

        private void ClickAction<T>(string propertyName)
            where T : ActionBase
        {
            var page = this.GetPageFromContext();

            var context = new ActionContext(propertyName.ToLookupKey());

            this.actionPipelineService
                    .PerformAction<T>(page, context)
                    .CheckResult();
        }
    }
}