import * as React from "react";
import { Provider } from "@fluentui/react-northstar";
import FeedbackPage from "../view-feedback/view-feedback-page";

import 'bootstrap/dist/css/bootstrap.min.css';
import "../../styles/feedback.css";
import { act } from "react-test-renderer";
import { render, unmountComponentAtNode } from "react-dom";
import pretty from 'pretty';

jest.mock('../../api/view-feedback-api');
jest.mock('react-i18next', () => ({
    useTranslation: () => ({
        t: (key: any) => key,
        i18n: { changeLanguage: jest.fn() }
    }),
    withTranslation: () => (Component: any) => {
        Component.defaultProps = { ...Component.defaultProps, t: (key: any) => key == "months" ? "January,February,March,April,May,June,July,August,September,October,November,December" : key };
        return Component;
    }
}));
jest.mock('react-spreadsheet', () => ({
    
}));


let container: any = null;
beforeEach(() => {
    // setup a DOM element as a render target
    container = document.createElement("div");
    // container *must* be attached to document so events work correctly.
    document.body.appendChild(container);
    
    act(() => {
        render(<Provider><FeedbackPage /></Provider>, container);
    });
});
afterEach(() => {
    // cleanup on exiting
    unmountComponentAtNode(container);
    container.remove();
    container = null;
});

describe('ViewFeedback', () => {
    it('renders snapshots', () => {
       expect(pretty(container.innerHTML)).toMatchSnapshot();
    });
});

