import { apiClient } from "./apiClient";
import axios from 'axios';

describe("apiClient", () => {
  it("should fetch data avoiding cors", async () => {
    const response = await axios.post(
      "https://1db54ls3q1.execute-api.ap-southeast-2.amazonaws.com/Stage/Word2Rtf",
      { Input: "【宣告/Proclaim】" },
      { headers: { "Content-Type": "application/json;charset=utf-8" } }
    );
    expect(response.status).toBe(200);
    expect(response.data.program.length).toBeGreaterThan(0);
  });
  
  it("should be empty object once the input is empty", async () => {
    const emptyResult = await apiClient();
    expect(emptyResult).toBe(null);
  });

  it("should return json object", async () => {
    let state = {
      output: []
    };
    state.output = await apiClient("【宣告/Proclaim】《詩篇/Psalm 50:23》");
    const { verses } = state.output[0];
    expect(verses.length).toBeGreaterThan(0);
  });

});
