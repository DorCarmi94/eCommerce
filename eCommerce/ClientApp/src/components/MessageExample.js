import React from "react";
export default ({ message }) => {
    return (
        <>
            <div className="media">
                <div className="media-body">
                    <p>{message}</p>
                </div>
            </div>
            <div className="dropdown-divider"></div>
        </>
    );
};