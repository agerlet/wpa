import axios from "axios";

export const apiClient = async input => {
  if (!input) return null;

  const response = await axios.post(
    process.env.REACT_APP_ENDPOINT,
    { Input: input },
    { headers: { "Content-Type": "application/json;charset=utf-8" } }
  );

  if (response.data.errorType) {
    throw response.data;
  }

  return response.data.program;
};
