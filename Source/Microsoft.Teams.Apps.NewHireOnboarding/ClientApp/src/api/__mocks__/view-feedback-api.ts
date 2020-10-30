// <copyright file="axios-decorator.ts" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

const iFeedbackDetail = {
    submittedOn: "2020-09-15T16:19:41.62824Z",
    feedback: "Test feedback",
    newHireName: "Bruno",
};

export const getFeedbackData = async (batchId: string)=> {
    console.log('Inside Mock...');
    return Promise.resolve({ data: { feedbackDetail: iFeedbackDetail } });
};