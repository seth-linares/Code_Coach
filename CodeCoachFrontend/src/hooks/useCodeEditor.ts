// src/hooks/useCodeEditor.ts

import {useState, useEffect, useCallback} from 'react';
import {EditorState, ProblemLanguageDetails} from "@/types";

// used to set Monaco editor's language
const languageMap: { [key: number]: string } = {
    51: 'csharp',  // C#
    54: 'cpp',     // C++
    92: 'python'   // Python
};



const useCodeEditor = (languageDetails: ProblemLanguageDetails[]) => {
    const [editorState, setEditorState] = useState<EditorState>({
        activeLanguage: null,
        codeByLanguage: {},
    });


    const [fontSize, setFontSize] = useState<number>(12);

    useEffect(() => {
        if (languageDetails.length > 0) {
            const initialState: EditorState = {
                activeLanguage: languageDetails[0].languageID, // Changed to languageID
                codeByLanguage: languageDetails.reduce((acc, lang) => {
                    acc[lang.languageID] = lang.functionSignature;
                    return acc;
                }, {} as Record<number, string>),
            };
            console.log(`activeLanguage: ${initialState.activeLanguage} - codeByLanguage: TESTTT HELLO!!`, initialState.codeByLanguage);
            setEditorState(initialState);
        }
    }, [languageDetails]);

    const updateCode = (newValue: string | undefined) => {
        const newCode = newValue ?? ""; // Handle undefined case
        setEditorState(prevState => {
            const updatedState = {
                ...prevState,
                codeByLanguage: {
                    ...prevState.codeByLanguage,
                    [prevState.activeLanguage!]: newCode,
                },
            };

            console.log(`${prevState.activeLanguage} set to:\n${newCode} PEEPPOOP`);
            return updatedState;
        });
    };




    const setActiveLanguage = useCallback((languageId: number) => {
        setEditorState(prevState => ({
            ...prevState,
            activeLanguage: languageId,
        }));
        console.log(`activeLanguage set to: ${languageId}`);
    }, []);


    // if languageDetails is not null and the active language is null, set the initial language
    useEffect(() => {
        if (languageDetails.length > 0 && !editorState.activeLanguage) {
            setActiveLanguage(languageDetails[0].languageID);
        }
        console.log(`Setting initial active language: ${languageDetails[0].languageID}`);
    }, [languageDetails, editorState.activeLanguage, setActiveLanguage]);


    // used to get the language name for the editor
    const getMonacoLanguage = (): string => {
        const activeLang: ProblemLanguageDetails | undefined = languageDetails.find(lang => lang.languageID === editorState.activeLanguage);
        return activeLang ? languageMap[activeLang.judge0LanguageId] : 'csharp';
    };


    return {
        editorState,
        setActiveLanguage,
        updateCode,
        getMonacoLanguage,
        fontSize,
        setFontSize,
    };
};

export default useCodeEditor;