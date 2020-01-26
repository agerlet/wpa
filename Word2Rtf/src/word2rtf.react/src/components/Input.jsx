import React from "react";

export const Input = props => (
  <div>
    <textarea
      id="input"
      data-testid="input"
      onChange={props.textareOnChange}
      defaultValue={props.input}
    ></textarea>
    <br />
    <input
      id="submit"
      data-testid="submit"
      type="submit"
      value="轉換"
      onClick={props.convertHandler}
    />
  </div>
);
