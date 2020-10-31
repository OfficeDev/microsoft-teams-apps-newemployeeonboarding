// <copyright file="view-feedback-api.ts" company="Microsoft">
// Copyright (c) Microsoft. All rights reserved.
// </copyright>

import axios from "./axios-decorator";
import { getBaseUrl } from '../configVariables';

let baseAxiosUrl = getBaseUrl() + '/api';

/**
* Get feedback data.
 * @param batchId Selected month and year to form batch id.
*/
export const getFeedbackData = async (batchId: string): Promise<any> => {
    let url = `${baseAxiosUrl}/feedback?batchId=${batchId}`;
    return await axios.get(url);
}