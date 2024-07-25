// src/hooks/useCodeEditor.ts
"use client";

import { useState, useEffect } from 'react';
import { ProblemLanguageDetails } from "@/types";

interface EditorState {
    activeLanguage: number | null;
    codeByLanguage: Record<number, string>;
}

const useCodeEditor = (languageDetails: ProblemLanguageDetails[]) => {
    const [editorState, setEditorState] = useState<EditorState>({
        activeLanguage: null,
        codeByLanguage: {},
    });

    useEffect(() => {
        if (languageDetails.length > 0) {
            const initialState: EditorState = {
                activeLanguage: languageDetails[0].languageID, // Changed to languageID
                codeByLanguage: languageDetails.reduce((acc, lang) => {
                    acc[lang.languageID] = lang.functionSignature;
                    return acc;
                }, {} as Record<number, string>),
            };
            console.log(`activeLanguage: ${initialState.activeLanguage} - codeByLanguage:`, initialState.codeByLanguage);
            setEditorState(initialState);
        }
    }, [languageDetails]);

    const setActiveLanguage = (languageId: number) => {
        setEditorState(prevState => ({
            ...prevState,
            activeLanguage: languageId,
        }));
        console.log(`activeLanguage set to: ${languageId}`);
    };

    const updateCode = (languageId: number, newCode: string) => {
        setEditorState(prevState => ({
            ...prevState,
            codeByLanguage: {
                ...prevState.codeByLanguage,
                [languageId]: newCode,
            },
        }));
    };

    const getActiveCode = (): string => {
        return editorState.activeLanguage !== null
            ? editorState.codeByLanguage[editorState.activeLanguage]
            : '';
    };

    const getEncodedActiveCode = (): string => {
        return btoa(getActiveCode());
    };

    return {
        activeLanguage: editorState.activeLanguage,
        codeByLanguage: editorState.codeByLanguage,
        setActiveLanguage,
        updateCode,
        getActiveCode,
        getEncodedActiveCode,
    };
};

export default useCodeEditor;