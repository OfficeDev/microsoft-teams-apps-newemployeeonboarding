// <copyright file="config-tab.tsx" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

import * as React from 'react';
import * as microsoftTeams from "@microsoft/teams-js";
import { WithTranslation, withTranslation } from "react-i18next";
import { TFunction } from "i18next";

export interface IConfigState {
    url: string;
}

class TeamsConfigPage extends React.Component<WithTranslation, IConfigState> {
    localize: TFunction;
    constructor(props: any) {
        super(props);
        this.localize = this.props.t;
        this.state = {
            url: this.getBaseUrl() + "/discover?theme={theme}&locale={locale}&teamId={teamId}&tenant={tid}"
        }
    }

    private getBaseUrl() {
        return window.location.origin;
    } 

    public componentDidMount() {
        microsoftTeams.initialize();
        microsoftTeams.settings.registerOnSaveHandler((saveEvent) => {
            microsoftTeams.settings.setSettings({
                entityId: "newemployeeonboarding_bot_app",
                contentUrl: this.state.url,
                websiteUrl: this.getBaseUrl(),
                suggestedDisplayName: this.localize("suggestedTabDisplayName"),
            });
            saveEvent.notifySuccess();
        });
        microsoftTeams.settings.setValidityState(true);
    }

    public render(): JSX.Element {
        return (
            <div className="container-div">
                <h3>{this.localize("configureTabMessage")}</h3>
            </div>
        );
    }
}

export default withTranslation()(TeamsConfigPage)