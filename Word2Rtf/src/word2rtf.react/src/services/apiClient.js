import axios from "axios";

export const apiClient = async input => {
  if (!input) return null;

  const response = await axios.post(
    "https://1db54ls3q1.execute-api.ap-southeast-2.amazonaws.com/Stage/Word2Rtf",
    { Input: input },
    { headers: { "Content-Type": "application/json;charset=utf-8" } }
  );

  if (response.data.errorType) {
    throw response.data;
  }

  return response.data.program;
};
