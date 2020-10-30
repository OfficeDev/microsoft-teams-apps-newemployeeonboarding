// <copyright file="download-feedback-page.tsx" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

import * as React from "react";
import { Loader } from "@fluentui/react-northstar";
import ReactExport from "react-data-export";
import { getFeedbackData } from "../../api/view-feedback-api";

import { withTranslation, WithTranslation } from "react-i18next";
import { TFunction } from "i18next";
import Resources from "../../constants/resources";

const ExcelFile = ReactExport.ExcelFile;
const ExcelSheet = ReactExport.ExcelFile.ExcelSheet;
const ExcelColumn = ReactExport.ExcelFile.ExcelColumn;

export interface IFeedbackDetails {
    submittedOn: string,
    feedback: string,
    newHireName: string,
}

interface IFeedbackState {
    isLoading: boolean;
    screenWidth: number;
    feedbackDetails: Array<IFeedbackDetails>
}

interface IFeedbackProps extends WithTranslation {
    closeDialog: (isOpen: boolean) => void;
    batchId: string;
}

class DownloadFeedback extends React.Component<IFeedbackProps, IFeedbackState>
{
    localize: TFunction;

    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        window.addEventListener("resize", this.update);
        this.state = {
            isLoading: true,
            screenWidth: 0,
            feedbackDetails: []
        }
    }

    /**
    * Used to initialize Microsoft Teams sdk.
    */
    componentDidMount() {
        this.setState({ isLoading: true });
        this.getFeedbackData(this.props.batchId);
        this.update();
    }

    /**
    * get screen width real time.
    */
    update = () => {
        this.setState({
            screenWidth: window.innerWidth
        });
    };

    /**
    * Fetch share feedback data.
    */
    getFeedbackData = async (batchId: string) => {
        let response = await getFeedbackData(batchId);
        if (response.status === 200 && response.data) {
            this.setState(
                {
                    feedbackDetails: response.data
                });
        }
        else {
            this.setState({
                feedbackDetails: [],
            });
        }

        this.setState({
            isLoading: false
        });
    }

    downloadFeedbacks() {
        if (this.state.isLoading) {
            return (
                <div className="container-div">
                    <div className="download-excel-container-subdiv">
                        <div className="download-excel-loader">
                            <Loader />
                        </div>
                    </div>
                </div>
            );
        }
        else {

            // Close dialog once excel data is prepared.
            setTimeout(() => {
                this.props.closeDialog(false);
            }, 0)

            return (
                <div>
                    <ExcelFile hideElement="false" filename={this.localize("feedbackExcelFileName")}>
                        <ExcelSheet data={this.state.feedbackDetails} name={this.localize("feedbackExcelSheetName")}>
                            <ExcelColumn label={this.localize("columnHeaderMonthText")} value={Resources.submittedOnColumnName} />
                            <ExcelColumn label={this.localize("columnHeaderNewHireNameText")} value={Resources.newHireColumnName} />
                            <ExcelColumn label={this.localize("columnHeaderFeedbackText")} value={Resources.feedbackColumnName} />
                        </ExcelSheet>
                    </ExcelFile>
                </div>
            );
        }
    }

    /**
    * Renders the component.
    */
    render(): JSX.Element {
        return (
            <div className="container-div">
                <div className="container-subdiv">
                    <div>
                        {this.downloadFeedbacks()}
                    </div>
                </div>
            </div>
        );
    }
}
export default withTranslation()(DownloadFeedback)